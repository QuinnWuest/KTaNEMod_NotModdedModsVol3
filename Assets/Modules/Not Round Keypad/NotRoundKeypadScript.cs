using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        "abbot", "abort", "about", "abuts", "after", "again", "aging", "alter", "apace", "argue", "aster", "barge", "beery",
        "below", "bight", "blare", "blown", "blows", "blowy", "bound", "bouts", "cater", "chink", "chose", "clean", "clear",
        "cloud", "colds", "could", "douse", "earns", "eater", "egret", "eight", "elbow", "emery", "ether", "every", "fairs",
        "fever", "fight", "fires", "firms", "first", "firth", "fists", "flare", "flirt", "foist", "found", "fount", "frond",
        "frost", "funds", "gains", "glace", "glare", "glean", "graft", "grain", "grant", "grate", "great", "greet", "hater",
        "heirs", "hinge", "hitch", "horse", "hosed", "hoses", "hound", "hours", "house", "hying", "joint", "laced", "laces",
        "lager", "lance", "large", "largo", "later", "leans", "leant", "learn", "leery", "lever", "light", "louse", "malls",
        "marge", "might", "mould", "mound", "mouse", "nerve", "never", "newer", "night", "ocher", "other", "otter", "outer",
        "paced", "paces", "pacey", "pagan", "paint", "panto", "pants", "pater", "peace", "pinto", "pints", "place", "plain",
        "plait", "plane", "plank", "plans", "plant", "plate", "pleat", "point", "posit", "pound", "print", "react", "resat",
        "right", "rites", "round", "rouse", "safer", "sally", "sarge", "scold", "sells", "sever", "shall", "shell", "shill",
        "sight", "sills", "silly", "skill", "slant", "small", "smell", "sneer", "sound", "souse", "space", "spell", "spelt",
        "spiel", "spill", "splat", "stall", "stile", "still", "stilt", "sting", "stink", "studs", "study", "sudsy", "swell",
        "swill", "sword", "tater", "tease", "tense", "terse", "thank", "their", "theme", "there", "therm", "these", "thick",
        "thigh", "thine", "thing", "think", "thins", "third", "thong", "those", "three", "threw", "tight", "tills", "tinge",
        "treat", "treed", "trees", "trill", "trite", "twill", "tying", "voter", "wader", "wafer", "wager", "waste", "water",
        "waver", "wheel", "where", "which", "white", "whore", "whorl", "whose", "winch", "witch", "wolds", "words", "wordy",
        "world", "would", "wound", "wrist", "write", "writs", "wrote", "yearn"
    };
    private string[] _wordBank;

    private readonly string _alphabet = "abcdefghijklmnopqrstuvwxyz";
    private readonly string[] _directions = new string[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
    private readonly string[] _directionNames = new string[] { "North", "North-East", "East", "South-East", "South", "South-West", "West", "North-West" };
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
    private SetPositionInfo[] _positionsNotPartOfSet;
    private int[] _buttonsPartOfSet;
    private int[] _buttonsNotPartOfSet;

    private SetType _setType;
    private TunnelPosition _startingTunnelPosition;
    private TunnelPosition _currentTunnelPosition;
    private TunnelPosition _goalTunnelPosition;

    private static readonly string[] _setTypeStrs = new string[4]
    {
        "took up the full cube's volume",
        "all shared an Y axis in common",
        "all shared an Z axis in common",
        "all shared an X axis in common",
    };

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
        _positionsNotPartOfSet = _positions.Skip(3).ToArray();
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

        Debug.LogFormat("[Not Round Keypad #{0}] The true north direction is oriented from the {1} button.", _moduleId, _directionNames[_northButton]);
        Debug.LogFormat("[Not Round Keypad #{0}] The semaphore of the non-SET buttons (with {1} being reoriented as North) spell out '{2}'.", _moduleId, _directionNames[_northButton], chosenWord);
        Debug.LogFormat("<Not Round Keypad #{0}> All letters: {1}", _moduleId, _allLetters);

        var initSet = _initialSets[_northButton - 1];
        Debug.LogFormat("[Not Round Keypad #{0}] The initial button set is {1}, {2}, {3}", _moduleId, _directions[initSet[0]], _directions[initSet[1]], _directions[initSet[2]]);
        int rotate = chosenIndex % 8;
        int startingSymbolIx = chosenIndex / 8;
        SetPositionInfo startingPosition = new SetPositionInfo(startingSymbolIx % 3, (startingSymbolIx / 3) % 3, (startingSymbolIx / 9) % 3);
        // Debug.LogFormat("[Not Round Keypad #{0}] The starting keypad symbol is {1}.", _moduleId, _charArr[startingSymbolIx]);
        var finalSet = new int[] { (initSet[0] + rotate) % 8, (initSet[1] + rotate) % 8, (initSet[2] + rotate) % 8 };
        Debug.LogFormat("[Not Round Keypad #{0}] The final button set is {1}, {2}, {3}", _moduleId, _directions[finalSet[0]], _directions[finalSet[1]], _directions[finalSet[2]]);

        var distinctLights = new List<int>();
        for (int f = 0; f < 3; f++)
        {
            var pair = _ledPairs[finalSet[f]];
            for (int p = 0; p < 2; p++)
                if (!distinctLights.Contains(pair[p]))
                    distinctLights.Add(pair[p]);
        }
        int distinctTotal = distinctLights.Count() - (distinctLights.Contains(_northButton) ? 1 : 0);
        Debug.LogFormat("[Not Round Keypad #{0}] The number of distinct light positions from the final set, ignoring the north button, is {1}", _moduleId, distinctTotal);
        var distances = finalSet.Select(z => _semaphoreDistances[_alphabet.IndexOf(_allLetters[z])]).ToArray();
        Array.Sort(distances);
        int medianDistance = distances[1];
        Debug.LogFormat("[Not Round Keypad #{0}] The median distance between lights from the final set is {1}.", _moduleId, medianDistance);
        Debug.LogFormat("<Not Round Keypad #{0}> All distances: {1}", _moduleId, distances.Join());

        _startingTunnelPosition = _startingTunnelPositions[medianDistance];
        _startingTunnelPosition.X = startingPosition.X;
        _startingTunnelPosition.Y = startingPosition.Y;
        _startingTunnelPosition.Z = startingPosition.Z;
        _startingTunnelPosition.KeypadSymbol = _charArr[startingSymbolIx];
        for (int cwRots = 0; cwRots < distinctTotal; cwRots++)
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
                Debug.LogFormat("[Not Round Keypad #{0}] Pressed the {1} button to go {2}.", _moduleId, _directionNames[i], dirPressed.ToString().ToUpperInvariant());
                _currentTunnelPosition = TunnelPosition.ApplyMovement(_currentTunnelPosition, dirPressed);
                if (!_currentTunnelPosition.IsValidTunnelPosition())
                {
                    Debug.LogFormat("Not Round Keypad {0}] However, this movement results in crashing into a wall. Strike. Resetting to starting position.", _moduleId);
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
        command = Regex.Replace(command.ToLowerInvariant().Trim(), @"^\s+", " ");
        yield break;
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        yield break;
    }
}
