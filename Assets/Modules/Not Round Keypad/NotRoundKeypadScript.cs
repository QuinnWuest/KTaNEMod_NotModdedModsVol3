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

    public TextMesh[] ButtonTexts;
    public KMSelectable[] ButtonSels;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

    private static readonly char[] _charList = new char[]
    {
        'Ϭ','Ѧ','Ѣ', 'Ͼ','æ','Ͽ', 'Ԇ','Ϙ','Җ',
        'Ϟ','¿','Ѯ', 'Ω','ټ','ƛ', '҂','Ѽ','Ѭ',
        '¶','★','Ҩ', 'ϗ','©','ψ', 'Ӭ','☆','Ҋ'
    };

    private readonly Dictionary<Position, char> _keypadSymbols = new Dictionary<Position, char>()
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

        _positions = GeneratePositions();
        _randomizedPositions = _positions.ToArray().Shuffle();
        _buttonChars = _randomizedPositions.Select(i => _keypadSymbols[i]).ToArray();
        for (int i = 0; i < _buttonChars.Length; i++)
            ButtonTexts[i].text = _buttonChars[i].ToString();
        Debug.LogFormat("[Not Round Keypad #{0}] Generated keypad symbols: {1}", _moduleId, _buttonChars.Join(", "));

        _positionsPartOfSet = _positions.Take(3).ToArray();
        _positionsNotPartOfSet = _positions.Skip(3).ToArray();
        _buttonsPartOfSet = Enumerable.Range(0, 8).Where(i => _positionsPartOfSet.Contains(_randomizedPositions[i])).ToArray();
        _buttonsNotPartOfSet = Enumerable.Range(0, 8).Except(_buttonsPartOfSet).ToArray();

        // do the rest
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

            // do code here
        };
    }

    private Action ButtonHighlightEnded(int i)
    {
        return delegate ()
        {
            if (_moduleSolved)
                return;

            // do code here
        };
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
