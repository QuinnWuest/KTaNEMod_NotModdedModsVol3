using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class NotRoundKeypadScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;

    int volumeSelect;
    int[] SETpositions = new int[3];
    int[] nonSETpositions = new int[5];

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

    private static readonly char[] _charList = new char[]{
    //x  0   1   2
    //y  0            1            2           //z
        'Ϭ','Ѧ','Ѣ', 'Ͼ','æ','Ͽ', 'Ԇ','Ϙ','Җ', //0
        'Ϟ','¿','Ѯ', 'Ω','ټ','ƛ', '҂','Ѽ','Ѭ', //1
        '¶','★','Ҩ', 'ϗ','©','ψ', 'Ӭ','☆','Ҋ' //2
    };

    private void Start()
    {
        _moduleId = _moduleIdCounter++;

        volumeSelect = Rnd.Range(0, 4);
        var rx = Enumerable.Range(0, 3).ToArray().Shuffle();
        var ry = Enumerable.Range(0, 3).ToArray().Shuffle();
        var rz = Enumerable.Range(0, 3).ToArray().Shuffle();
        switch (volumeSelect)
        {
            case 0: SETpositions = new int[] { rx[0]+ry[0]*3+rz[0]*9, rx[0]+ry[1]*3+rz[1]*9, rx[0]+ry[2]*3+rz[2]*9 }; break;
            case 1: SETpositions = new int[] { rx[0]+ry[0]*3+rz[0]*9, rx[1]+ry[0]*3+rz[1]*9, rx[2]+ry[0]*3+rz[2]*9 }; break;
            case 2: SETpositions = new int[] { rx[0]+ry[0]*3+rz[0]*9, rx[1]+ry[1]*3+rz[0]*9, rx[2]+ry[2]*3+rz[0]*9 }; break;
            case 3: SETpositions = new int[] { rx[0]+ry[0]*3+rz[0]*9, rx[1]+ry[1]*3+rz[1]*9, rx[2]+ry[2]*3+rz[2]*9 }; break;
        }
        
    }

#pragma warning disable 0414
    private readonly string TwitchHelpMessage = @"";
#pragma warning restore 0414

    private IEnumerator ProcessTwitchCommand (string command)
    {
        command = Regex.Replace(command.ToLowerInvariant().Trim(), @"^\s+", " ");
        yield break;
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        yield break;
    }
}
