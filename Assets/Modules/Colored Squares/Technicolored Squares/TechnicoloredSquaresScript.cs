using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;

public partial class TechnicoloredSquaresScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;
    public KMRuleSeedable RuleSeedable;
    public KMColorblindMode ColorblindMode;

    public GameObject[] SquareObjs;
    public Light[] SquareLights;
    public KMSelectable[] SquareSels;

    public Material[] SquareMats;
    public Material[] SquareCBMats;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

    private bool _colorblindMode;
    private Coroutine _activeCoroutine;

    private static readonly SquareColor[] _colorsInUse =
    {
        SquareColor.Red,
        SquareColor.Yellow,
        SquareColor.Green,
        SquareColor.Blue,
        SquareColor.Magenta
    };

    private static readonly Color[] _lightColors = new Color[] { Color.black, Color.white, Color.red, Color.yellow, Color.green, new Color32(0x83, 0x83, 0xff, 0xff), Color.magenta };

    private SquareColor[] _squareColorsInternal;
    private SquareColor[] _squareColorsOnModule;
    private SquareColor _gridColor;

    private SquareGridTransformation _grid = new SquareGridTransformation();

    // Rule seeded stuff
    private bool[][] _bitDiagrams = new bool[5][] { new bool[16], new bool[16], new bool[16], new bool[16], new bool[16] };
    private LogicGate[] _logicGates = new LogicGate[5];
    private GridRule[] _rules;

    // End rule seeded stuff

    private List<int[]> _solutionPairs = new List<int[]>();
    private List<int> _validSquaresToPress = new List<int>();
    private int _activePair = -1;

    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        _colorblindMode = ColorblindMode.ColorblindModeActive;

        // START RULE SEED
        var rnd = RuleSeedable.GetRNG();
        if (rnd.Seed != 1)
            Debug.LogFormat("[Technicolored Squares #{0}] Using rule seed {1}.", _moduleId, rnd.Seed);
        for (int d = 0; d < _bitDiagrams.Length; d++)
        {
            var bits = new bool[16];
            for (int i = 0; i < 8; i++)
                bits[i] = true;
            _bitDiagrams[d] = rnd.ShuffleFisherYates(bits).ToArray();
        }
        for (int d = 0; d < _bitDiagrams.Length; d++)
            Debug.LogFormat("[Technicolored Squares #{0}] {1} bit diagram: {2}", _moduleId, _colorsInUse[d], FormatGrid(_bitDiagrams[d]));

        var gates = new LogicGate[] { LogicGate.AND, LogicGate.OR, LogicGate.NAND, LogicGate.NOR, LogicGate.XOR, LogicGate.XNOR };
        _logicGates = rnd.ShuffleFisherYates(gates).Take(5).ToArray();
        for (int i = 0; i < _logicGates.Length; i++)
            Debug.LogFormat("[Technicolored Squares #{0}] {1} uses {2}.", _moduleId, _colorsInUse[i], _logicGates[i]);

        var rules = (GridRule[])Enum.GetValues(typeof(GridRule));
        _rules = rnd.ShuffleFisherYates(rules).ToArray();
        // END RULE SEED

        for (int i = 0; i < SquareSels.Length; i++)
            SquareSels[i].OnInteract += SquarePress(i);

        Setup();
    }

    private void Setup()
    {
        _activePair = -1;
        GenerateGrid();
        _solutionPairs = GenerateSolution();
        _validSquaresToPress.Clear();
        for (int i = 0; i < _solutionPairs.Count; i++)
            _validSquaresToPress.Add(_solutionPairs[i][0]);
        StartSquareColorsCoroutine(_squareColorsOnModule, delay: true);
    }

    private void GenerateGrid()
    {
        _gridColor = _colorsInUse[Rnd.Range(0, _colorsInUse.Length)];
        var list = new List<SquareColor>();
        foreach (var color in _colorsInUse)
        {
            int count = color == _gridColor ? 8 : 2;
            for (int j = 0; j < count; j++)
                list.Add(color);
        }
        _squareColorsInternal = list.ToArray().Shuffle();
        _squareColorsOnModule = _squareColorsInternal.ToArray();

        Debug.LogFormat("[Technicolored Squares #{0}] Grid color is {1}.", _moduleId, _gridColor);
        Debug.LogFormat("[Technicolored Squares #{0}] {1}.", _moduleId, FormatColors(_squareColorsInternal));
    }

    private List<int[]> GenerateSolution()
    {
        var pairedSquares = Enumerable.Range(0, 16).Where(i => _squareColorsInternal[i] != _gridColor).ToArray();
        Debug.LogFormat("[Technicolored Squares #{0}] Paired squares in reading order: {1}.", _moduleId, FormatPositions(pairedSquares));
        var gridColorIndex = Array.IndexOf(_colorsInUse, _gridColor);
        var workingGrid = _bitDiagrams[gridColorIndex].ToArray();
        Debug.LogFormat("[Technicolored Squares #{0}] Starting with the {1} bit diagram: {2}.", _moduleId, _gridColor, FormatGrid(workingGrid));

        for (int i = 0; i < pairedSquares.Length; i++)
        {
            int position = pairedSquares[i];
            var color = _squareColorsInternal[position];
            var colorIx = Array.IndexOf(_colorsInUse, color);
            var rule = _rules[colorIx * 16 + position];

            Debug.LogFormat("[Technicolored Squares #{0}] Square {1} is at {2}; applying {3}.", _moduleId, GetCoord(position), color, rule);

            _grid.ApplyTransformation(workingGrid, rule, pairedSquares);

            Debug.LogFormat("[Technicolored Squares #{0}] Grid is now: {1}.", _moduleId, FormatGrid(workingGrid));
        }

        var result = new List<int[]>();

        var handledColors = new List<SquareColor>();

        for (int i = 0; i < pairedSquares.Length; i++)
        {
            int first = pairedSquares[i];
            var color = _squareColorsInternal[first];
            if (handledColors.Contains(color))
                continue;
            handledColors.Add(color);
            int second = pairedSquares.First(p => p > first && _squareColorsInternal[p] == color);
            var colorIx = Array.IndexOf(_colorsInUse, color);
            bool firstBit = workingGrid[first];
            bool secondBit = workingGrid[second];
            var gate = _logicGates[colorIx];
            bool gateResult = ApplyLogicGate(gate, firstBit, secondBit);
            if (gateResult)
                result.Add(new int[] { first, second });
            else
                result.Add(new int[] { second, first });

            Debug.LogFormat("[Technicolored Squares #{0}] {1} pair is squares at {2} and {3}: {4} {5} {6} = {7}. Press {8} then {9}.", _moduleId,
                color, GetCoord(first), GetCoord(second), firstBit ? 1 : 0, gate, secondBit ? 1 : 0, gateResult ? "true" : "false",
                gateResult ? GetCoord(first) : GetCoord(second),
                gateResult ? GetCoord(second) : GetCoord(first)
            );
        }
        return result;
    }

    private KMSelectable.OnInteractHandler SquarePress(int i)
    {
        return delegate ()
        {
            PlaySound(i);
            SquareSels[i].AddInteractionPunch();
            if (_moduleSolved)
                return false;

            _squareColorsOnModule[i] = SquareColor.White;
            SetButtonColor(i, SquareColor.White);

            if (_activePair == -1)
            {
                int pIx = _solutionPairs.FindIndex(p => p[0] == i);
                if (pIx == -1)
                {
                    Debug.LogFormat("[Technicolored Squares #{0}] Incorrectly pressed {1}. Strike.", _moduleId, GetCoord(i));
                    Strike();
                    return false;
                }
                Debug.LogFormat("[Technicolored Squares #{0}] Correctly pressed {1}.", _moduleId, GetCoord(i));
                _activePair = pIx;
                _validSquaresToPress.Clear();
                _validSquaresToPress.Add(_solutionPairs[_activePair][1]);

                return false;
            }

            if (i != _solutionPairs[_activePair][1])
            {
                Debug.LogFormat("[Technicolored Squares #{0}] Incorrectly pressed {1}. Strike.", _moduleId, GetCoord(i));
                Strike();
                return false;
            }

            Debug.LogFormat("[Technicolored Squares #{0}] Correctly pressed {1}.", _moduleId, GetCoord(i));
            _solutionPairs.RemoveAt(_activePair);
            _activePair = -1;

            _validSquaresToPress.Clear();

            for (int j = 0; j < _solutionPairs.Count; j++)
                _validSquaresToPress.Add(_solutionPairs[j][0]);

            if (_solutionPairs.Count == 0)
            {
                Debug.LogFormat("[Technicolored Squares #{0}] Module solved!", _moduleId);
                _moduleSolved = true;
                SetAllButtonsBlack();
                Module.HandlePass();
            }

            return false;
        };
    }

    private void Strike()
    {
        Module.HandleStrike();
        SetAllButtonsBlack();
        Setup();
    }

    private bool ApplyLogicGate(LogicGate gate, bool a, bool b)
    {
        switch (gate)
        {
            case LogicGate.AND:
                return a && b;
            case LogicGate.OR:
                return a || b;
            case LogicGate.NAND:
                return !(a && b);
            case LogicGate.NOR:
                return !(a || b);
            case LogicGate.XOR:
                return a != b;
            case LogicGate.XNOR:
                return a == b;
            default:
                throw new InvalidOperationException("Invalid logic gate");
        }
    }

    private GridRule GetRule(SquareColor color, int position)
    {
        int colorIndex = Array.IndexOf(_colorsInUse, color);
        return _rules[colorIndex * 16 + position];
    }

    private void PlaySound(int index)
    {
        switch (_squareColorsOnModule[index])
        {
            case SquareColor.Red:
            case SquareColor.Black:
            case SquareColor.White:
                Audio.PlaySoundAtTransform("ColoredSquaresRed", SquareSels[index].transform);
                break;
            case SquareColor.Blue:
                Audio.PlaySoundAtTransform("ColoredSquaresBlue", SquareSels[index].transform);
                break;
            case SquareColor.Green:
                Audio.PlaySoundAtTransform("ColoredSquaresGreen", SquareSels[index].transform);
                break;
            case SquareColor.Yellow:
                Audio.PlaySoundAtTransform("ColoredSquaresYellow", SquareSels[index].transform);
                break;
            case SquareColor.Magenta:
                Audio.PlaySoundAtTransform("ColoredSquaresMagenta", SquareSels[index].transform);
                break;
        }
    }

    private void SetButtonColor(int ix, SquareColor color)
    {
        if (color == SquareColor.Black)
            SetButtonBlack(ix);
        else
        {
            SquareObjs[ix].GetComponent<MeshRenderer>().sharedMaterial = _colorblindMode ? SquareCBMats[(int)color] ?? SquareMats[(int)color] : SquareMats[(int)color];
            SquareLights[ix].color = _lightColors[(int)color];
            SquareLights[ix].gameObject.SetActive(true);
        }
    }

    private void SetButtonBlack(int ix)
    {
        SquareObjs[ix].GetComponent<MeshRenderer>().sharedMaterial = SquareMats[(int)SquareColor.Black];
        SquareLights[ix].gameObject.SetActive(false);
    }

    private void SetAllButtonsBlack()
    {
        for (int i = 0; i < 16; i++)
            SetButtonBlack(i);
    }

    private void StartSquareColorsCoroutine(SquareColor[] colors, SquaresToRecolor behaviour = SquaresToRecolor.All, bool delay = false, bool unshuffled = false)
    {
        var indexes = new List<int?>((behaviour == SquaresToRecolor.NonwhiteOnly
            ? Enumerable.Range(0, 16).Where(ix => colors[ix] != SquareColor.White)
            : behaviour == SquaresToRecolor.NonblackOnly
            ? Enumerable.Range(0, 16).Where(ix => colors[ix] != SquareColor.Black)
            : Enumerable.Range(0, 16)).Select(i => (int?)i));
        if (!unshuffled)
            indexes.Shuffle();
        if (delay)
            indexes.Insert(0, null);
        StartSquareColorsCoroutine(colors, indexes.ToArray());
    }

    private void StartSquareColorsCoroutine(SquareColor[] colors, int?[] indexes)
    {
        if (_activeCoroutine != null)
            StopCoroutine(_activeCoroutine);
        _activeCoroutine = StartCoroutine(SetSquareColorsCoroutine(colors, indexes));
    }

    private IEnumerator SetSquareColorsCoroutine(SquareColor[] colors, int?[] indexes)
    {
        foreach (var i in indexes)
        {
            if (i == null)
                yield return new WaitForSeconds(Rnd.Range(1.5f, 2f));
            else
            {
                SetButtonColor(i.Value, colors[i.Value]);
                yield return new WaitForSeconds(.03f);
            }
        }
        _activeCoroutine = null;
    }

    private string GetCoord(int position)
    {
        return "ABCD"[position % 4].ToString() + "1234"[position / 4].ToString();
    }

    private string FormatGrid(bool[] grid)
    {
        var rows = new string[4];

        for (int row = 0; row < 4; row++)
        {
            var chars = new char[4];

            for (int col = 0; col < 4; col++)
                chars[col] = grid[row * 4 + col] ? '1' : '0';

            rows[row] = new string(chars);
        }

        return rows.Join(" / ");
    }

    private string FormatColors(SquareColor[] colors)
    {
        return colors.Select(c => c.ToString()).ToArray().Join(", ");
    }

    private string FormatPositions(IEnumerable<int> positions)
    {
        return positions.Select(i => GetCoord(i).ToString()).ToArray().Join(", ");
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} A1 A2 A3 B3 [specify column as letter, then row as number] | !{0} colorblind";
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        if (command.Trim().Equals("colorblind", StringComparison.InvariantCultureIgnoreCase))
        {
            _colorblindMode = !_colorblindMode;
            StartSquareColorsCoroutine(_squareColorsOnModule);
            yield return null;
            yield break;
        }

        var buttons = new List<KMSelectable>();
        foreach (var piece in command.ToLowerInvariant().Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (piece.Length != 2 || piece[0] < 'a' || piece[0] > 'd' || piece[1] < '1' || piece[1] > '4')
                yield break;
            buttons.Add(SquareSels[(piece[0] - 'a') + 4 * (piece[1] - '1')]);
        }

        yield return null;
        yield return "solve";
        yield return "strike";
        yield return buttons;
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        while (!_moduleSolved)
        {
            var button = _validSquaresToPress.PickRandom();
            SquareSels[button].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}