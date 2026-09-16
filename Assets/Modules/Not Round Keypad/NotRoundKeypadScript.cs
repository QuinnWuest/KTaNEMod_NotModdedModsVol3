using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;

public partial class NotRoundKeypadScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;
    public KMRuleSeedable RuleSeedable;

    public TextMesh[] ButtonTexts;
    public KMSelectable[] ButtonSels;
    public GameObject[] ButtonLEDs;
    public Material[] ButtonLEDMats;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

    private static readonly char[] _fullCharList = new char[] { 'Ϭ', 'Ѧ', 'Ѣ', 'Ͼ', 'æ', 'Ͽ', 'Ԇ', 'Ϙ', 'Җ', 'Ϟ', '¿', 'Ѯ', 'Ω', 'ټ', 'ƛ', '҂', 'Ѽ', 'Ѭ', '¶', '★', 'Ҩ', 'ϗ', '©', 'ψ', 'Ӭ', '☆', 'Ҋ' };
    private static readonly int[] _ogCharIxs = { 11, 14, 26, 3, 21, 13, 9, 0, 20, 12, 5, 7, 15, 18, 1, 24, 25, 8, 17, 10, 19, 23, 2, 4, 6, 16, 22 };

    private int[] _charIxs;
    private char[] _charArr;

    private Dictionary<SetPositionInfo, char> _keypadSymbolDict = new Dictionary<SetPositionInfo, char>();

    //words deliberately chosen to cause potential ambiguity when full sequence of letters is read; derived from _Password_'s word list
    private static readonly string[] _fullWordBank = new string[] {
        "ABBOT", "ABORT", "ABOUT", "ABUTS", "AFTER", "AGAIN", "AGING", "ALTER", "APACE", "ARGUE", "ASTER", "BARGE", "BEERY",
        "BELOW", "BIGHT", "BLARE", "BLOWN", "BLOWS", "BLOWY", "BOUND", "BOUTS", "CATER", "CHINK", "CHOSE", "CLEAN", "CLEAR",
        "CLOUD", "COLDS", "COULD", "DOUSE", "EARNS", "EATER", "EGRET", "EIGHT", "ELBOW", "EMERY", "ETHER", "EVERY", "FAIRS",
        "FEVER", "FIGHT", "FIRES", "FIRMS", "FIRST", "FIRTH", "FISTS", "FLARE", "FLIRT", "FOIST", "FOUND", "FOUNT", "FROND",
        "FROST", "FUNDS", "GAINS", "GLACE", "GLARE", "GLEAN", "GRAFT", "GRAIN", "GRANT", "GRATE", "GREAT", "GREET", "HATER",
        "HEIRS", "HINGE", "HITCH", "HORSE", "HOSED", "HOSES", "HOUND", "HOURS", "HOUSE", "HYING", "JOINT", "LACED", "LACES",
        "LAGER", "LANCE", "LARGE", "LARGO", "LATER", "LEANS", "LEANT", "LEARN", "LEERY", "LEVER", "LIGHT", "LOUSE", "MALLS",
        "MARGE", "MIGHT", "MOULD", "MOUND", "MOUSE", "NERVE", "NEVER", "NEWER", "NIGHT", "OCHER", "OTHER", "OTTER", "OUTER",
        "PACED", "PACES", "PACEY", "PAGAN", "PAINT", "PANTO", "PANTS", "PATER", "PEACE", "PINTO", "PINTS", "PLACE", "PLAIN",
        "PLAIT", "PLANE", "PLANK", "PLANS", "PLANT", "PLATE", "PLEAT", "POINT", "POSIT", "POUND", "PRINT", "REACT", "RESAT",
        "RIGHT", "RITES", "ROUND", "ROUSE", "SAFER", "SALLY", "SARGE", "SCOLD", "SELLS", "SEVER", "SHALL", "SHELL", "SHILL",
        "SIGHT", "SILLS", "SILLY", "SKILL", "SLANT", "SMALL", "SMELL", "SNEER", "SOUND", "SOUSE", "SPACE", "SPELL", "SPELT",
        "SPIEL", "SPILL", "SPLAT", "STALL", "STILE", "STILL", "STILT", "STING", "STINK", "STUDS", "STUDY", "SUDSY", "SWELL",
        "SWILL", "SWORD", "TATER", "TEASE", "TENSE", "TERSE", "THANK", "THEIR", "THEME", "THERE", "THERM", "THESE", "THICK",
        "THIGH", "THINE", "THING", "THINK", "THINS", "THIRD", "THONG", "THOSE", "THREE", "THREW", "TIGHT", "TILLS", "TINGE",
        "TREAT", "TREED", "TREES", "TRILL", "TRITE", "TWILL", "TYING", "VOTER", "WADER", "WAFER", "WAGER", "WASTE", "WATER",
        "WAVER", "WHEEL", "WHERE", "WHICH", "WHITE", "WHORE", "WHORL", "WHOSE", "WINCH", "WITCH", "WOLDS", "WORDS", "WORDY",
        "WORLD", "WOULD", "WOUND", "WRIST", "WRITE", "WRITS", "WROTE", "YEARN"
    };
    private string[] _wordBank;

    private readonly string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private readonly string[] _directions = new string[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
    private readonly string[] _directionNames = new string[] { "NORTH", "NORTH-EAST", "EAST", "SOUTH-EAST", "SOUTH", "SOUTH-WEST", "WEST", "NORTH-WEST" };
    private static readonly string[] _setTypeStrs = new string[4]
    {
        "took up the full cube's volume",
        "all shared an Y axis in common",
        "all shared an Z axis in common",
        "all shared an X axis in common",
    };

    private int _northButton;

    private static readonly TunnelPosition[] _startingTunnelPositions = new TunnelPosition[]
    {
        new TunnelPosition(facingWall: 1, upWall: 2, rightWall: 3),
        new TunnelPosition(facingWall: 2, upWall: 3, rightWall: 1),
        new TunnelPosition(facingWall: 3, upWall: 1, rightWall: 2),
        new TunnelPosition(facingWall: 4, upWall: 6, rightWall: 5),
        new TunnelPosition(facingWall: 5, upWall: 4, rightWall: 6),
        new TunnelPosition(facingWall: 6, upWall: 5, rightWall: 4),
    };

    private readonly int[][] _semaphore =
    {
        new int[] { 4, 5 }, new int[] { 4, 6 }, new int[] { 4, 7 }, new int[] { 0, 4 }, new int[] { 1, 4 },
        new int[] { 2, 4 }, new int[] { 3, 4 }, new int[] { 5, 6 }, new int[] { 5, 7 }, new int[] { 0, 2 },
        new int[] { 0, 5 }, new int[] { 1, 5 }, new int[] { 2, 5 }, new int[] { 3, 5 }, new int[] { 6, 7 },
        new int[] { 0, 6 }, new int[] { 1, 6 }, new int[] { 2, 6 }, new int[] { 3, 6 }, new int[] { 0, 7 },
        new int[] { 1, 7 }, new int[] { 0, 3 }, new int[] { 1, 2 }, new int[] { 1, 3 }, new int[] { 2, 7 }, new int[] { 2, 3 }
    };
    private readonly int[] _semaphoreDistances =
    {
        1, 2, 3, 4, 3, 2, 1, 1, 2, 2, 3, 4, 3, 2, 1, 2, 3, 4, 3, 1, 2, 3, 1, 2, 3, 1
    };

    private readonly int[][] _initialSets =
    {
        new int[] { 0, 1, 6 }, new int[] { 0, 2, 5 }, new int[] { 0, 1, 5 }, new int[] { 0, 1, 2 }, new int[] { 0, 1, 4 }, new int[] { 0, 2, 4 }, new int[] { 0, 1, 3 }
    };
    private string _allLetters;
    private int[][] _ledPairs = {
        new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }, new int[] { -1, -1 }
    };

    private SetPositionInfo[] _positions;
    private SetPositionInfo[] _randomizedPositions;
    private char[] _buttonChars;

    private SetPositionInfo[] _positionsPartOfSet;
    private int[] _buttonsPartOfSet;
    private int[] _buttonsNotPartOfSet;

    private SetType _setType;
    private TunnelPosition _startingTunnelPosition;
    private TunnelPosition _currentTunnelPosition;
    private TunnelPosition _goalTunnelPosition;

    private void Start()
    {
        // more general TODO: apparently underscores everywhere doesn't quite emulate your coding style Quinn, fix how you see fit
        // @blan: I use underscores for fields, but no underscores for local variables, but I use uppercase starting letters for public fields to assign in the inspector
        _moduleId = _moduleIdCounter++;

        for (int i = 0; i < ButtonSels.Length; i++)
        {
            ButtonSels[i].OnInteract += ButtonPress(i);
            ButtonSels[i].OnHighlight += ButtonHighlight(i);
            ButtonSels[i].OnHighlightEnded += ButtonHighlightEnded(i);
        }

        // START RULE SEED
        var rnd = RuleSeedable.GetRNG();
        _wordBank = _fullWordBank.ToArray();
        rnd.ShuffleFisherYates(_wordBank);
        _charIxs = _ogCharIxs.ToArray();
        rnd.ShuffleFisherYates(_charIxs);
        _charArr = _charIxs.Select(i => _fullCharList[i]).ToArray();

        _keypadSymbolDict.Clear();
        for (int x = 0; x < 3; x++)
            for (int y = 0; y < 3; y++)
                for (int z = 0; z < 3; z++)
                    _keypadSymbolDict.Add(new SetPositionInfo(x, y, z), _charArr[x + y * 3 + z * 9]);
        // END RULE SEED

        _positions = GeneratePositions();
        if (_positions[0].X == _positions[1].X)
            _setType = SetType.SameXAxis;
        else if (_positions[0].Y == _positions[1].Y)
            _setType = SetType.SameYAxis;
        else if (_positions[0].Z == _positions[1].Z)
            _setType = SetType.SameZAxis;
        else
            _setType = SetType.Cube;

        _randomizedPositions = _positions.ToArray().Shuffle();
        _buttonChars = _randomizedPositions.Select(i => _keypadSymbolDict[i]).ToArray();
        for (int i = 0; i < _buttonChars.Length; i++)
            ButtonTexts[i].text = _buttonChars[i].ToString();
        Debug.LogFormat("[Not Round Keypad #{0}] Keypad symbols: {1}", _moduleId, _buttonChars.Join(", "));

        _positionsPartOfSet = _positions.Take(3).ToArray();
        _buttonsPartOfSet = Enumerable.Range(0, 8).Where(i => _positionsPartOfSet.Contains(_randomizedPositions[i])).ToArray();
        _buttonsNotPartOfSet = Enumerable.Range(0, 8).Except(_buttonsPartOfSet).ToArray();

        int chosenIndex = Rnd.Range(0, 208);
        string chosenWord = _wordBank[chosenIndex];

        _allLetters = ObtainAllLetters(chosenWord, _buttonsNotPartOfSet, _buttonsPartOfSet);
        _northButton = Rnd.Range(1, 8);
        for (int b = 0; b < 8; b++)
        {
            var sem = _semaphore[_alphabet.IndexOf(_allLetters[b])];
            _ledPairs[b] = new int[] { (sem[0] + _northButton) % 8, (sem[1] + _northButton) % 8 };
            Debug.LogFormat("[Not Round Keypad #{0}] ♦ The {1} button's LEDs: {2}, {3}", _moduleId, _directions[b], _directions[_ledPairs[b][0]], _directions[_ledPairs[b][1]]);
        }
        Debug.LogFormat("[Not Round Keypad #{0}] The SET symbols are: {1}, {2}, {3}", _moduleId, _buttonChars[_buttonsPartOfSet[0]], _buttonChars[_buttonsPartOfSet[1]], _buttonChars[_buttonsPartOfSet[2]]);
        Debug.LogFormat("[Not Round Keypad #{0}] The symbols of the SET {1}.", _moduleId, _setTypeStrs[(int)_setType]);

        Debug.LogFormat("[Not Round Keypad #{0}] The true North direction is oriented from the {1} button.", _moduleId, _directionNames[_northButton]);
        Debug.LogFormat("[Not Round Keypad #{0}] The semaphore of the non-SET buttons (with {1} being reoriented as North) spell out \"{2}\".", _moduleId, _directionNames[_northButton], chosenWord);
        Debug.LogFormat("<Not Round Keypad #{0}> All letters: {1}", _moduleId, _allLetters);

        var initSet = _initialSets[_northButton - 1];
        Debug.LogFormat("[Not Round Keypad #{0}] The initial button set is {1}, {2}, {3}.", _moduleId, _directions[initSet[0]], _directions[initSet[1]], _directions[initSet[2]]);
        int rotate = chosenIndex % 8;
        int startingSymbolIx = chosenIndex / 8;
        SetPositionInfo startingPosition = new SetPositionInfo(startingSymbolIx % 3, (startingSymbolIx / 3) % 3, (startingSymbolIx / 9) % 3);
        // Debug.LogFormat("[Not Round Keypad #{0}] The starting keypad symbol is {1}.", _moduleId, _charArr[startingSymbolIx]);
        var finalSet = new int[] { (initSet[0] + rotate) % 8, (initSet[1] + rotate) % 8, (initSet[2] + rotate) % 8 };
        Debug.LogFormat("[Not Round Keypad #{0}] The final button set is {1}, {2}, {3}.", _moduleId, _directions[finalSet[0]], _directions[finalSet[1]], _directions[finalSet[2]]);

        var distinctLights = new List<int>();
        for (int f = 0; f < 3; f++)
        {
            var pair = _ledPairs[finalSet[f]];
            for (int p = 0; p < 2; p++)
                if (!distinctLights.Contains(pair[p]))
                    distinctLights.Add(pair[p]);
        }
        int distinctTotal = distinctLights.Count() - (distinctLights.Contains(_northButton) ? 1 : 0);
        Debug.LogFormat("[Not Round Keypad #{0}] The number of distinct light positions from the final set, ignoring the north button, is {1}.", _moduleId, distinctTotal);
        var distances = finalSet.Select(z => _semaphoreDistances[_alphabet.IndexOf(_allLetters[z])]).ToArray();
        Array.Sort(distances);
        int medianDistance = distances[1];
        Debug.LogFormat("[Not Round Keypad #{0}] The median distance between lights from the final set is {1}.", _moduleId, medianDistance);
        Debug.LogFormat("<Not Round Keypad #{0}> All distances: {1}.", _moduleId, distances.Join());

        var startingOrientation = _startingTunnelPositions[distinctTotal - 1];
        _startingTunnelPosition = new TunnelPosition(x: startingPosition.X, y: startingPosition.Y, z: startingPosition.Z, facingWall: startingOrientation.FacingWall, upWall: startingOrientation.UpWall, rightWall: startingOrientation.RightWall, kps: _charArr[startingSymbolIx]);
        for (int cwRots = 0; cwRots < medianDistance; cwRots++)
            _startingTunnelPosition = TunnelPosition.ApplyMovement(_startingTunnelPosition, TunnelDirection.Clockwise);
        int goalIx = Array.IndexOf(_charArr, _buttonChars[_northButton]);
        _goalTunnelPosition = new TunnelPosition(x: goalIx % 3, y: (goalIx / 3) % 3, z: (goalIx / 9) % 3, kps: _buttonChars[_northButton]);

        Debug.LogFormat("[Not Round Keypad #{0}] Starting position: {1}", _moduleId, _startingTunnelPosition.ToStringWithWalls());
        Debug.LogFormat("[Not Round Keypad #{0}] Goal position: {1}", _moduleId, _goalTunnelPosition.ToStringWithoutWalls());

        ResetToStartingPosition();
    }

    private void ResetToStartingPosition()
    {
        _currentTunnelPosition = new TunnelPosition(_startingTunnelPosition.X, _startingTunnelPosition.Y, _startingTunnelPosition.Z, _startingTunnelPosition.FacingWall, _startingTunnelPosition.UpWall, _startingTunnelPosition.RightWall, _startingTunnelPosition.KeypadSymbol);
    }

    private KMSelectable.OnInteractHandler ButtonPress(int i)
    {
        return delegate ()
        {
            ButtonSels[i].AddInteractionPunch();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ButtonSels[i].transform);
            if (_moduleSolved)
                return false;

            int modulo = i % 2;
            if (modulo == 0)
            {
                TunnelDirection dirPressed = (TunnelDirection)(i / 2);
                Debug.LogFormat("[Not Round Keypad #{0}] Pressed {1} button to go {2}.", _moduleId, _directionNames[i], dirPressed.ToString().ToUpperInvariant());
                _currentTunnelPosition = TunnelPosition.ApplyMovement(_currentTunnelPosition, dirPressed);
                if (!_currentTunnelPosition.IsValidTunnelPosition())
                {
                    Debug.LogFormat("[Not Round Keypad {0}] However, this movement results in crashing into a wall. Strike. Resetting to starting position.", _moduleId);
                    ResetToStartingPosition();
                    for (int b = 0; b < 8; b++)
                        ButtonLEDs[b].GetComponent<MeshRenderer>().material = ButtonLEDMats[2];
                    Module.HandleStrike();
                    return false;
                }
                _currentTunnelPosition.KeypadSymbol = _keypadSymbolDict[new SetPositionInfo(_currentTunnelPosition.X.Value, _currentTunnelPosition.Y.Value, _currentTunnelPosition.Z.Value)];
                Debug.LogFormat("[Not Round Keypad #{0}] Current position: {1}", _moduleId, _currentTunnelPosition.ToStringWithWalls());
            }
            else
            {
                SetType setTypePressed = (SetType)(i / 2);
                Debug.LogFormat("[Not Round Keypad #{0}] Attempted to submit using the {1} button, indicating that the SET {2}.", _moduleId, _directionNames[i], _setTypeStrs[(int)setTypePressed]);

                bool sameSetType = _setType == setTypePressed;
                bool samePosition = _currentTunnelPosition.Equals(_goalTunnelPosition);

                if (sameSetType && samePosition)
                {
                    Debug.LogFormat("[Not Round Keypad #{0}] You submitted at the goal position using the correct button. Module solved.", _moduleId);
                    for (int led = 0; led < 8; led++)
                        ButtonLEDs[led].GetComponent<MeshRenderer>().material = ButtonLEDMats[3];
                    _moduleSolved = true;
                    Module.HandlePass();
                    return false;
                }
                if (sameSetType && !samePosition)
                    Debug.LogFormat("[Not Round Keypad #{0}] You submitted with the correct button, but you were not at the goal position. Strike. Resetting to starting position.", _moduleId);
                if (!sameSetType && samePosition)
                    Debug.LogFormat("[Not Round Keypad #{0}] You submitted at the goal position, but not with the correct button. Strike. Resetting to starting position.", _moduleId);
                if (!sameSetType && !samePosition)
                    Debug.LogFormat("[Not Round Keypad #{0}] You submitted neither at the goal position nor used the correct button. Strike. Resetting to starting position.", _moduleId);
                ResetToStartingPosition();
                for (int b = 0; b < 8; b++)
                    ButtonLEDs[b].GetComponent<MeshRenderer>().material = ButtonLEDMats[2];
                Module.HandleStrike();
                return false;
            }
            return false;
        };
    }

    private Action ButtonHighlight(int i)
    {
        return delegate ()
        {
            if (_moduleSolved)
                return;
            for (int b = 0; b < 8; b++)
                ButtonLEDs[b].GetComponent<MeshRenderer>().material = ButtonLEDMats[0];
            ButtonLEDs[_ledPairs[i][0]].GetComponent<MeshRenderer>().material = ButtonLEDMats[1];
            ButtonLEDs[_ledPairs[i][1]].GetComponent<MeshRenderer>().material = ButtonLEDMats[1];
        };
    }

    private Action ButtonHighlightEnded(int i)
    {
        return delegate ()
        {
            if (_moduleSolved)
                return;

            for (int b = 0; b < 8; b++)
                ButtonLEDs[b].GetComponent<MeshRenderer>().material = ButtonLEDMats[0];
        };
    }

    private string ObtainAllLetters(string w, int[] n, int[] y)
    {
        char[] chars = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        for (int i = 0; i < 5; i++)
            chars[n[i]] = w[i];

        for (int j = 0; j < 3; j++)
        {
            List<int> p = new List<int>();
            for (int f = 0; f < 5; f++)
            {
                string l = "";
                string r = "";
                for (int h = 0; h < 5; h++)
                {
                    if (f != h)
                    {
                        if (n[h] < y[j])
                        {
                            l += w[h];
                        }
                        else
                        {
                            r += w[h];
                        }
                    }
                }
                for (int b = 0; b < 26; b++)
                {
                    if (!p.Contains(b))
                    {
                        if (_wordList.Contains(l + _alphabet[b] + r))
                        {
                            p.Add(b);
                        }
                    }
                }
            }
            int d = Enumerable.Range(0, 26).Except(p).PickRandom();
            p.Add(d);
            chars[y[j]] = _alphabet[p.PickRandom()];
        }

        return chars.Join("");
    }

    private SetPositionInfo[] GeneratePositions()
    {
        while (true)
        {
            var positions = new List<SetPositionInfo>();

            SetPositionInfo a = new SetPositionInfo(Rnd.Range(0, 3), Rnd.Range(0, 3), Rnd.Range(0, 3));

            SetPositionInfo b;
            do { b = new SetPositionInfo(Rnd.Range(0, 3), Rnd.Range(0, 3), Rnd.Range(0, 3)); }
            while (a.SharesTwoAxes(b));

            SetPositionInfo c = SetPositionInfo.GetThirdToFormSet(a, b);

            positions.Add(a);
            positions.Add(b);
            positions.Add(c);

            while (positions.Count < 8)
            {
                var candidates = new List<SetPositionInfo>();
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            var candidate = new SetPositionInfo(x, y, z);

                            if (positions.Contains(candidate))
                                continue;

                            bool createsSet = false;

                            for (int i = 0; i < positions.Count && !createsSet; i++)
                            {
                                for (int j = i + 1; j < positions.Count; j++)
                                {
                                    var third = SetPositionInfo.GetThirdToFormSet(positions[i], positions[j]);
                                    if (candidate.Equals(third))
                                    {
                                        createsSet = true;
                                        break;
                                    }
                                }
                            }
                            if (!createsSet)
                                candidates.Add(candidate);
                        }
                    }
                }
                if (candidates.Count == 0)
                    break;

                var next = candidates[Rnd.Range(0, candidates.Count)];
                positions.Add(next);
            }
            if (positions.Count == 8)
                return positions.ToArray();
        }
    }

#pragma warning disable 0414
    private readonly string TwitchHelpMessage = @"";
#pragma warning restore 0414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.Trim().ToLowerInvariant();
        var parameters = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var list = new List<int>();
        if (parameters.Length < 2)
            yield break;

        for (int i = 1; i < parameters.Length; i++)
        {
            switch (parameters[i])
            {
                case "up":
                case "u":
                case "north":
                case "n":
                case "1":
                    list.Add(0);
                    break;
                case "upright":
                case "ur":
                case "northeast":
                case "ne":
                case "2":
                    list.Add(1);
                    break;
                case "right":
                case "r":
                case "east":
                case "e":
                case "3":
                    list.Add(2);
                    break;
                case "upleft":
                case "ul":
                case "northwest":
                case "nw":
                case "4":
                    list.Add(3);
                    break;
                case "down":
                case "d":
                case "south":
                case "s":
                case "5":
                    list.Add(4);
                    break;
                case "downright":
                case "dr":
                case "southeast":
                case "se":
                case "6":
                    list.Add(5);
                    break;
                case "left":
                case "l":
                case "west":
                case "w":
                case "7":
                    list.Add(6);
                    break;
                case "downleft":
                case "dl":
                case "southwest":
                case "sw":
                case "8":
                    list.Add(7);
                    break;
                default:
                    yield break;
            }
        }
        switch (parameters[0])
        {
            case "move":
                if (list.Any(btn => btn % 2 != 0))
                {
                    yield return "sendtochaterror You cannot move diagonally. Command ignored.";
                    yield break;
                }
                foreach (var btn in list)
                {
                    ButtonSels[btn].OnInteract();
                    yield return new WaitForSeconds(0.2f);
                }
                break;
            case "submit":
                if (list.Count != 1)
                {
                    yield return "sendtochaterror You cannot submit more than one button. Command ignored.";
                    yield break;
                }
                if (list[0] % 2 == 0)
                {
                    yield return "sendtochaterror You cannot submit on a non-diagonal button. Command ignored.";
                    yield break;
                }
                ButtonSels[list[0]].OnInteract();
                break;
            case "highlight":
            case "hl":
                foreach (var btn in list)
                {
                    ButtonSels[btn].OnHighlight();
                    yield return new WaitForSeconds(1.25f);
                    ButtonSels[btn].OnHighlightEnded();
                    yield return new WaitForSeconds(0.25f);
                }
                break;
            default:
                yield break;
        }
    }

    struct QueueItem
    {
        public TunnelPosition Position { get; private set; }
        public TunnelPosition Parent { get; private set; }
        public TunnelDirection Direction { get; private set; }

        public QueueItem(TunnelPosition position, TunnelPosition parent, TunnelDirection direction)
        {
            Position = position;
            Parent = parent;
            Direction = direction;
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        if (_currentTunnelPosition.Equals(_goalTunnelPosition))
            goto found;
        var visited = new Dictionary<TunnelPosition, QueueItem>();
        var q = new Queue<QueueItem>();
        q.Enqueue(new QueueItem(_currentTunnelPosition, null, TunnelDirection.Up));
        while (q.Count > 0)
        {
            var qi = q.Dequeue();
            if (visited.ContainsKey(qi.Position))
                continue;
            visited[qi.Position] = qi;
            if (qi.Position.Equals(_goalTunnelPosition))
                break;
            var up = TunnelPosition.ApplyMovement(qi.Position, TunnelDirection.Up);
            var right = TunnelPosition.ApplyMovement(qi.Position, TunnelDirection.Right);
            var down = TunnelPosition.ApplyMovement(qi.Position, TunnelDirection.Down);
            var left = TunnelPosition.ApplyMovement(qi.Position, TunnelDirection.Left);
            if (up.IsValidTunnelPosition())
                q.Enqueue(new QueueItem(up, qi.Position, TunnelDirection.Up));
            if (right.IsValidTunnelPosition())
                q.Enqueue(new QueueItem(right, qi.Position, TunnelDirection.Right));
            if (down.IsValidTunnelPosition())
                q.Enqueue(new QueueItem(down, qi.Position, TunnelDirection.Down));
            if (left.IsValidTunnelPosition())
                q.Enqueue(new QueueItem(left, qi.Position, TunnelDirection.Left));
        }
        var r = _goalTunnelPosition;
        var path = new List<TunnelDirection>();
        while (true)
        {
            var nr = visited[r];
            if (nr.Parent == null)
                break;
            path.Add(nr.Direction);
            r = nr.Parent;
        }
        for (int i = path.Count - 1; i >= 0; i--)
        {
            ButtonSels[(int)path[i] * 2].OnInteract();
            yield return new WaitForSeconds(0.2f);
        }
        found:;
        ButtonSels[(int)_setType * 2 + 1].OnInteract();
        yield break;
    }
}
