public partial class TechnicoloredSquaresScript
{
    public enum GridRule
    {
        SwapRow1Row2,
        SwapRow1Row3,
        SwapRow1Row4,
        SwapRow2Row3,
        SwapRow2Row4,
        SwapRow3Row4,

        SwapColumnAColumnB,
        SwapColumnAColumnC,
        SwapColumnAColumnD,
        SwapColumnBColumnC,
        SwapColumnBColumnD,
        SwapColumnCColumnD,

        InvertRow1,
        InvertRow2,
        InvertRow3,
        InvertRow4,

        InvertColumnA,
        InvertColumnB,
        InvertColumnC,
        InvertColumnD,

        InvertTopLeftQuadrant,
        InvertTopRightQuadrant,
        InvertBottomLeftQuadrant,
        InvertBottomRightQuadrant,

        InvertCentralFour,
        InvertFourCorners,

        ShiftRow1Left,
        ShiftRow2Left,
        ShiftRow3Left,
        ShiftRow4Left,

        ShiftRow1Right,
        ShiftRow2Right,
        ShiftRow3Right,
        ShiftRow4Right,

        ShiftColumnAUp,
        ShiftColumnBUp,
        ShiftColumnCUp,
        ShiftColumnDUp,

        ShiftColumnADown,
        ShiftColumnBDown,
        ShiftColumnCDown,
        ShiftColumnDDown,

        FlipRow1,
        FlipRow2,
        FlipRow3,
        FlipRow4,

        FlipColumnA,
        FlipColumnB,
        FlipColumnC,
        FlipColumnD,

        SwapLeftRightHalves,
        SwapTopBottomHalves,

        SwapTopLeftTopRight,
        SwapTopLeftBottomLeft,
        SwapTopLeftBottomRight,
        SwapTopRightBottomLeft,
        SwapTopRightBottomRight,
        SwapBottomLeftBottomRight,

        RotateTopLeftClockwise,
        RotateTopLeftCounterclockwise,
        RotateTopRightClockwise,
        RotateTopRightCounterclockwise,
        RotateBottomLeftClockwise,
        RotateBottomLeftCounterclockwise,
        RotateBottomRightClockwise,
        RotateBottomRightCounterclockwise,

        RotateCentralClockwise,
        RotateCentralCounterclockwise,

        RotateCornersClockwise,
        RotateCornersCounterclockwise,

        RotateGridClockwise,
        RotateGridCounterclockwise,

        InvertPairedSquare1,
        InvertPairedSquare2,
        InvertPairedSquare3,
        InvertPairedSquare4,
        InvertPairedSquare5,
        InvertPairedSquare6,
        InvertPairedSquare7,
        InvertPairedSquare8
    }
}