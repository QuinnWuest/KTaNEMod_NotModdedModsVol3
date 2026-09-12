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

    private char[] _charList = new char[]
    {
        'Ϭ','Ѧ','Ѣ', 'Ͼ','æ','Ͽ', 'Ԇ','Ϙ','Җ',
        'Ϟ','¿','Ѯ', 'Ω','ټ','ƛ', '҂','Ѽ','Ѭ',
        '¶','★','Ҩ', 'ϗ','©','ψ', 'Ӭ','☆','Ҋ'
    };

    private Dictionary<Position, char> _keypadSymbols = new Dictionary<Position, char>()
    {
        [new Position(0, 0, 0)] = 'Ϭ',
        [new Position(1, 0, 0)] = 'Ѧ',
        [new Position(2, 0, 0)] = 'Ѣ',
        [new Position(0, 1, 0)] = 'Ͼ',
        [new Position(1, 1, 0)] = 'æ',
        [new Position(2, 1, 0)] = 'Ͽ',
        [new Position(0, 2, 0)] = 'Ԇ',
        [new Position(1, 2, 0)] = 'Ϙ',
        [new Position(2, 2, 0)] = 'Җ',

        [new Position(0, 0, 1)] = 'Ϟ',
        [new Position(1, 0, 1)] = '¿',
        [new Position(2, 0, 1)] = 'Ѯ',
        [new Position(0, 1, 1)] = 'Ω',
        [new Position(1, 1, 1)] = 'ټ',
        [new Position(2, 1, 1)] = 'ƛ',
        [new Position(0, 2, 1)] = '҂',
        [new Position(1, 2, 1)] = 'Ѽ',
        [new Position(2, 2, 1)] = 'Ѭ',

        [new Position(0, 0, 2)] = '¶',
        [new Position(1, 0, 2)] = '★',
        [new Position(2, 0, 2)] = 'Ҩ',
        [new Position(0, 1, 2)] = 'ϗ',
        [new Position(1, 1, 2)] = '©',
        [new Position(2, 1, 2)] = 'ψ',
        [new Position(0, 2, 2)] = 'Ӭ',
        [new Position(1, 2, 2)] = '☆',
        [new Position(2, 2, 2)] = 'Ҋ'
    };

    //words deliberately chosen to cause potential ambiguity when full sequence of letters is read; derived from _Password_'s word list
    private string[] _wordBank = new string[] {
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
    private readonly string _alphabet = "abcdefghijklmnopqrstuvwxyz";
    private int _northButton;

    private readonly int[][] _semaphore =
    {
        new int[] { 4, 5 }, new int[] { 4, 6 }, new int[] { 4, 7 }, new int[] { 0, 4 }, new int[] { 1, 4 }, 
        new int[] { 2, 4 }, new int[] { 3, 4 }, new int[] { 5, 6 }, new int[] { 5, 7 }, new int[] { 0, 2 }, 
        new int[] { 0, 5 }, new int[] { 1, 5 }, new int[] { 2, 5 }, new int[] { 3, 5 }, new int[] { 6, 7 }, 
        new int[] { 0, 6 }, new int[] { 1, 6 }, new int[] { 2, 6 }, new int[] { 3, 6 }, new int[] { 0, 7 }, 
        new int[] { 1, 7 }, new int[] { 0, 3 }, new int[] { 1, 2 }, new int[] { 1, 3 }, new int[] { 2, 7 }, new int[] { 2, 3 }
    };

    private readonly int[][] _initialSets =
    {
        new int[] { 0, 1, 6 }, new int[] { 0, 2, 5 }, new int[] { 0, 1, 5 }, new int[] { 0, 1, 2 }, new int[] { 0, 1, 4 }, new int[] { 0, 2, 4 }, new int[] { 0, 1, 3 }
    };
    private string _allLetters;
    
    private Position[] _positions;
    private Position[] _randomizedPositions;
    private char[] _buttonChars;

    private Position[] _positionsPartOfSet;
    private Position[] _positionsNotPartOfSet;
    private int[] _buttonsPartOfSet;
    private int[] _buttonsNotPartOfSet;

    private void Start()
    {
        _moduleId = _moduleIdCounter++;

        for (int i = 0; i < ButtonSels.Length; i++)
        {
            ButtonSels[i].OnInteract += ButtonPress(i);
            ButtonSels[i].OnHighlight += ButtonHighlight(i);
            ButtonSels[i].OnHighlightEnded += ButtonHighlightEnded(i);
        }

        var ruleseed = RuleSeedable.GetRNG();

        _positions = GeneratePositions();
        _randomizedPositions = _positions.ToArray().Shuffle();
        _buttonChars = _randomizedPositions.Select(i => _keypadSymbols[i]).ToArray();
        for (int i = 0; i < _buttonChars.Length; i++)
            ButtonTexts[i].text = _buttonChars[i].ToString();
        Debug.LogFormat("[Not Round Keypad #{0}] Keypad symbols: {1}", _moduleId, _buttonChars.Join(", "));

        _positionsPartOfSet = _positions.Take(3).ToArray();
        _positionsNotPartOfSet = _positions.Skip(3).ToArray();
        _buttonsPartOfSet = Enumerable.Range(0, 8).Where(i => _positionsPartOfSet.Contains(_randomizedPositions[i])).ToArray();
        _buttonsNotPartOfSet = Enumerable.Range(0, 8).Except(_buttonsPartOfSet).ToArray();

        var _shuffledBank = ruleseed.ShuffleFisherYates(_wordBank);
        int _chosenIndex = Rnd.Range(0, 208);
        string _chosenWord = _shuffledBank[_chosenIndex];
        
        _allLetters = ObtainAllLetters(_chosenWord, _buttonsNotPartOfSet, _buttonsPartOfSet);
        _northButton = Rnd.Range(1, 8);
        var _initSet = _initialSets[_northButton - 1];
        int _rotate = _chosenIndex % 8;
        int _startingSymbol = _chosenIndex / 8;
        var _finalSet = new int[] { (_initSet[0] + _rotate) % 8, (_initSet[1] + _rotate) % 8, (_initSet[2] + _rotate) % 8 };
    
        
    }

    private KMSelectable.OnInteractHandler ButtonPress(int i)
    {
        return delegate ()
        {
            ButtonSels[i].AddInteractionPunch();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ButtonSels[i].transform);
            if (_moduleSolved)
                return false;

            // do code here

            return false;
        };
    }

    private Action ButtonHighlight(int i)
    {
        return delegate ()
        {
            if (_moduleSolved)
                return;

            var sem = _semaphore[_alphabet.IndexOf(_allLetters[i])];
            var rot = new int[] { (sem[0] + _northButton) % 8, (sem[1] + _northButton) % 8 };
            ButtonLEDs[rot[0]].GetComponent<MeshRenderer>().material = ButtonLEDMats[1];
            ButtonLEDs[rot[1]].GetComponent<MeshRenderer>().material = ButtonLEDMats[1];
        };
    }

    private Action ButtonHighlightEnded(int i)
    {
        return delegate ()
        {
            if (_moduleSolved)
                return;

            for (int b = 0; b < 8; b++)
            {
                ButtonLEDs[b].GetComponent<MeshRenderer>().material = ButtonLEDMats[0];
            }
        };
    }

    private string ObtainAllLetters(string w, int[] n, int[] y)
    {
        char[] chars = { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        for (int i = 0; i < 5; i++)
        {
            chars[n[i]] = w[i];
        }

        //how not to code
        //string[] fours = { "", "", "", "", "" }; for (int a = 0; a < 5; a++) { char c = w[a]; for (int b = 0; b < 5; b++) { if (a != b) fours[b] += c; } }

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
                        } else
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
            int d = -1;
            do
            {
                d = Rnd.Range(0, 26);
            } while (p.Contains(d));
            p.Add(d);
            chars[y[j]] = _alphabet[p.PickRandom()];
        }

        return chars.Join("");
    }

    private Position[] GeneratePositions()
    {
        while (true)
        {
            var positions = new List<Position>();

            Position a = new Position(Rnd.Range(0, 3), Rnd.Range(0, 3), Rnd.Range(0, 3));

            Position b;
            do { b = new Position(Rnd.Range(0, 3), Rnd.Range(0, 3), Rnd.Range(0, 3)); }
            while (a.Equals(b));

            Position c = Position.GetThirdToFormSet(a, b);

            positions.Add(a);
            positions.Add(b);
            positions.Add(c);

            while (positions.Count < 8)
            {
                var candidates = new List<Position>();
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        for (int z = 0; z < 3; z++)
                        {
                            var candidate = new Position(x, y, z);

                            if (positions.Contains(candidate))
                                continue;

                            bool createsSet = false;

                            for (int i = 0; i < positions.Count && !createsSet; i++)
                            {
                                for (int j = i + 1; j < positions.Count; j++)
                                {
                                    var third = Position.GetThirdToFormSet(positions[i], positions[j]);
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
