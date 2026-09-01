public partial class TechnicoloredSquaresScript
{
    public class SquareGridTransformation
    {
        public void ApplyTransformation(bool[] grid, GridRule rule, int[] pairedSquares)
        {
            switch (rule)
            {
                // Swap rows
                case GridRule.SwapRow1Row2:
                    SwapRows(grid, 0, 1);
                    break;

                case GridRule.SwapRow1Row3:
                    SwapRows(grid, 0, 2);
                    break;

                case GridRule.SwapRow1Row4:
                    SwapRows(grid, 0, 3);
                    break;

                case GridRule.SwapRow2Row3:
                    SwapRows(grid, 1, 2);
                    break;

                case GridRule.SwapRow2Row4:
                    SwapRows(grid, 1, 3);
                    break;

                case GridRule.SwapRow3Row4:
                    SwapRows(grid, 2, 3);
                    break;


                // Swap columns
                case GridRule.SwapColumnAColumnB:
                    SwapColumns(grid, 0, 1);
                    break;

                case GridRule.SwapColumnAColumnC:
                    SwapColumns(grid, 0, 2);
                    break;

                case GridRule.SwapColumnAColumnD:
                    SwapColumns(grid, 0, 3);
                    break;

                case GridRule.SwapColumnBColumnC:
                    SwapColumns(grid, 1, 2);
                    break;

                case GridRule.SwapColumnBColumnD:
                    SwapColumns(grid, 1, 3);
                    break;

                case GridRule.SwapColumnCColumnD:
                    SwapColumns(grid, 2, 3);
                    break;


                // Invert rows
                case GridRule.InvertRow1:
                    InvertRow(grid, 0);
                    break;

                case GridRule.InvertRow2:
                    InvertRow(grid, 1);
                    break;

                case GridRule.InvertRow3:
                    InvertRow(grid, 2);
                    break;

                case GridRule.InvertRow4:
                    InvertRow(grid, 3);
                    break;


                // Invert columns
                case GridRule.InvertColumnA:
                    InvertColumn(grid, 0);
                    break;

                case GridRule.InvertColumnB:
                    InvertColumn(grid, 1);
                    break;

                case GridRule.InvertColumnC:
                    InvertColumn(grid, 2);
                    break;

                case GridRule.InvertColumnD:
                    InvertColumn(grid, 3);
                    break;


                // Invert quadrants
                case GridRule.InvertTopLeftQuadrant:
                    InvertBlock(grid, 0, 0);
                    break;

                case GridRule.InvertTopRightQuadrant:
                    InvertBlock(grid, 0, 2);
                    break;

                case GridRule.InvertBottomLeftQuadrant:
                    InvertBlock(grid, 2, 0);
                    break;

                case GridRule.InvertBottomRightQuadrant:
                    InvertBlock(grid, 2, 2);
                    break;

                case GridRule.InvertCentralFour:
                    InvertBlock(grid, 1, 1);
                    break;

                case GridRule.InvertFourCorners:
                    InvertCorners(grid);
                    break;


                // Shift rows left
                case GridRule.ShiftRow1Left:
                    ShiftRow(grid, 0, false);
                    break;

                case GridRule.ShiftRow2Left:
                    ShiftRow(grid, 1, false);
                    break;

                case GridRule.ShiftRow3Left:
                    ShiftRow(grid, 2, false);
                    break;

                case GridRule.ShiftRow4Left:
                    ShiftRow(grid, 3, false);
                    break;


                // Shift rows right
                case GridRule.ShiftRow1Right:
                    ShiftRow(grid, 0, true);
                    break;

                case GridRule.ShiftRow2Right:
                    ShiftRow(grid, 1, true);
                    break;

                case GridRule.ShiftRow3Right:
                    ShiftRow(grid, 2, true);
                    break;

                case GridRule.ShiftRow4Right:
                    ShiftRow(grid, 3, true);
                    break;


                // Shift columns up
                case GridRule.ShiftColumnAUp:
                    ShiftColumn(grid, 0, false);
                    break;

                case GridRule.ShiftColumnBUp:
                    ShiftColumn(grid, 1, false);
                    break;

                case GridRule.ShiftColumnCUp:
                    ShiftColumn(grid, 2, false);
                    break;

                case GridRule.ShiftColumnDUp:
                    ShiftColumn(grid, 3, false);
                    break;


                // Shift columns down
                case GridRule.ShiftColumnADown:
                    ShiftColumn(grid, 0, true);
                    break;

                case GridRule.ShiftColumnBDown:
                    ShiftColumn(grid, 1, true);
                    break;

                case GridRule.ShiftColumnCDown:
                    ShiftColumn(grid, 2, true);
                    break;

                case GridRule.ShiftColumnDDown:
                    ShiftColumn(grid, 3, true);
                    break;


                // Flip rows
                case GridRule.FlipRow1:
                    FlipRow(grid, 0);
                    break;

                case GridRule.FlipRow2:
                    FlipRow(grid, 1);
                    break;

                case GridRule.FlipRow3:
                    FlipRow(grid, 2);
                    break;

                case GridRule.FlipRow4:
                    FlipRow(grid, 3);
                    break;


                // Flip columns
                case GridRule.FlipColumnA:
                    FlipColumn(grid, 0);
                    break;

                case GridRule.FlipColumnB:
                    FlipColumn(grid, 1);
                    break;

                case GridRule.FlipColumnC:
                    FlipColumn(grid, 2);
                    break;

                case GridRule.FlipColumnD:
                    FlipColumn(grid, 3);
                    break;


                // Swap halves
                case GridRule.SwapLeftRightHalves:
                    SwapLeftRightHalves(grid);
                    break;

                case GridRule.SwapTopBottomHalves:
                    SwapTopBottomHalves(grid);
                    break;


                // Swap quadrants
                case GridRule.SwapTopLeftTopRight:
                    SwapBlocks(grid, 0, 0, 0, 2);
                    break;

                case GridRule.SwapTopLeftBottomLeft:
                    SwapBlocks(grid, 0, 0, 2, 0);
                    break;

                case GridRule.SwapTopLeftBottomRight:
                    SwapBlocks(grid, 0, 0, 2, 2);
                    break;

                case GridRule.SwapTopRightBottomLeft:
                    SwapBlocks(grid, 0, 2, 2, 0);
                    break;

                case GridRule.SwapTopRightBottomRight:
                    SwapBlocks(grid, 0, 2, 2, 2);
                    break;

                case GridRule.SwapBottomLeftBottomRight:
                    SwapBlocks(grid, 2, 0, 2, 2);
                    break;


                // Rotate quadrants
                case GridRule.RotateTopLeftClockwise:
                    Rotate2x2(grid, 0, 0, true);
                    break;

                case GridRule.RotateTopLeftCounterclockwise:
                    Rotate2x2(grid, 0, 0, false);
                    break;

                case GridRule.RotateTopRightClockwise:
                    Rotate2x2(grid, 0, 2, true);
                    break;

                case GridRule.RotateTopRightCounterclockwise:
                    Rotate2x2(grid, 0, 2, false);
                    break;

                case GridRule.RotateBottomLeftClockwise:
                    Rotate2x2(grid, 2, 0, true);
                    break;

                case GridRule.RotateBottomLeftCounterclockwise:
                    Rotate2x2(grid, 2, 0, false);
                    break;

                case GridRule.RotateBottomRightClockwise:
                    Rotate2x2(grid, 2, 2, true);
                    break;

                case GridRule.RotateBottomRightCounterclockwise:
                    Rotate2x2(grid, 2, 2, false);
                    break;


                // Rotate center
                case GridRule.RotateCentralClockwise:
                    Rotate2x2(grid, 1, 1, true);
                    break;

                case GridRule.RotateCentralCounterclockwise:
                    Rotate2x2(grid, 1, 1, false);
                    break;


                // Rotate corners
                case GridRule.RotateCornersClockwise:
                    RotateCorners(grid, true);
                    break;

                case GridRule.RotateCornersCounterclockwise:
                    RotateCorners(grid, false);
                    break;


                // Rotate whole grid
                case GridRule.RotateGridClockwise:
                    RotateGrid(grid, true);
                    break;

                case GridRule.RotateGridCounterclockwise:
                    RotateGrid(grid, false);
                    break;


                // Invert paired squares
                case GridRule.InvertPairedSquare1:
                    InvertPosition(grid, pairedSquares[0]);
                    break;

                case GridRule.InvertPairedSquare2:
                    InvertPosition(grid, pairedSquares[1]);
                    break;

                case GridRule.InvertPairedSquare3:
                    InvertPosition(grid, pairedSquares[2]);
                    break;

                case GridRule.InvertPairedSquare4:
                    InvertPosition(grid, pairedSquares[3]);
                    break;

                case GridRule.InvertPairedSquare5:
                    InvertPosition(grid, pairedSquares[4]);
                    break;

                case GridRule.InvertPairedSquare6:
                    InvertPosition(grid, pairedSquares[5]);
                    break;

                case GridRule.InvertPairedSquare7:
                    InvertPosition(grid, pairedSquares[6]);
                    break;

                case GridRule.InvertPairedSquare8:
                    InvertPosition(grid, pairedSquares[7]);
                    break;
            }
        }

        private void Swap(bool[] grid, int a, int b)
        {
            bool temp = grid[a];
            grid[a] = grid[b];
            grid[b] = temp;
        }


        private void SwapRows(bool[] grid, int row1, int row2)
        {
            for (int col = 0; col < 4; col++)
                Swap(grid, row1 * 4 + col, row2 * 4 + col);
        }


        private void SwapColumns(bool[] grid, int col1, int col2)
        {
            for (int row = 0; row < 4; row++)
                Swap(grid, row * 4 + col1, row * 4 + col2);
        }


        private void InvertRow(bool[] grid, int row)
        {
            for (int col = 0; col < 4; col++)
            {
                int ix = row * 4 + col;
                grid[ix] = !grid[ix];
            }
        }


        private void InvertColumn(bool[] grid, int col)
        {
            for (int row = 0; row < 4; row++)
            {
                int ix = row * 4 + col;
                grid[ix] = !grid[ix];
            }
        }


        private void InvertBlock(bool[] grid, int row, int col)
        {
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    int ix = (row + r) * 4 + (col + c);
                    grid[ix] = !grid[ix];
                }
            }
        }


        private void InvertCorners(bool[] grid)
        {
            grid[0] = !grid[0];
            grid[3] = !grid[3];
            grid[12] = !grid[12];
            grid[15] = !grid[15];
        }


        private void InvertPosition(bool[] grid, int position)
        {
            grid[position] = !grid[position];
        }


        private void ShiftRow(bool[] grid, int row, bool right)
        {
            int ix = row * 4;

            bool a = grid[ix];
            bool b = grid[ix + 1];
            bool c = grid[ix + 2];
            bool d = grid[ix + 3];

            if (right)
            {
                grid[ix] = d;
                grid[ix + 1] = a;
                grid[ix + 2] = b;
                grid[ix + 3] = c;
            }
            else
            {
                grid[ix] = b;
                grid[ix + 1] = c;
                grid[ix + 2] = d;
                grid[ix + 3] = a;
            }
        }


        private void ShiftColumn(bool[] grid, int col, bool down)
        {
            bool a = grid[col];
            bool b = grid[4 + col];
            bool c = grid[8 + col];
            bool d = grid[12 + col];

            if (down)
            {
                grid[col] = d;
                grid[4 + col] = a;
                grid[8 + col] = b;
                grid[12 + col] = c;
            }
            else
            {
                grid[col] = b;
                grid[4 + col] = c;
                grid[8 + col] = d;
                grid[12 + col] = a;
            }
        }


        private void FlipRow(bool[] grid, int row)
        {
            int ix = row * 4;

            Swap(grid, ix, ix + 3);
            Swap(grid, ix + 1, ix + 2);
        }


        private void FlipColumn(bool[] grid, int col)
        {
            Swap(grid, col, 12 + col);
            Swap(grid, 4 + col, 8 + col);
        }


        private void SwapLeftRightHalves(bool[] grid)
        {
            for (int row = 0; row < 4; row++)
            {
                int ix = row * 4;

                Swap(grid, ix, ix + 2);
                Swap(grid, ix + 1, ix + 3);
            }
        }


        private void SwapTopBottomHalves(bool[] grid)
        {
            SwapRows(grid, 0, 2);
            SwapRows(grid, 1, 3);
        }


        private void SwapBlocks(
            bool[] grid,
            int row1,
            int col1,
            int row2,
            int col2)
        {
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    int first = (row1 + r) * 4 + (col1 + c);
                    int second = (row2 + r) * 4 + (col2 + c);

                    Swap(grid, first, second);
                }
            }
        }


        private void Rotate2x2(
            bool[] grid,
            int row,
            int col,
            bool clockwise)
        {
            int topLeft = row * 4 + col;
            int topRight = topLeft + 1;
            int bottomLeft = topLeft + 4;
            int bottomRight = bottomLeft + 1;

            bool a = grid[topLeft];
            bool b = grid[topRight];
            bool c = grid[bottomLeft];
            bool d = grid[bottomRight];

            if (clockwise)
            {
                grid[topLeft] = c;
                grid[topRight] = a;
                grid[bottomRight] = b;
                grid[bottomLeft] = d;
            }
            else
            {
                grid[topLeft] = b;
                grid[topRight] = d;
                grid[bottomRight] = c;
                grid[bottomLeft] = a;
            }
        }


        private void RotateCorners(bool[] grid, bool clockwise)
        {
            bool topLeft = grid[0];
            bool topRight = grid[3];
            bool bottomRight = grid[15];
            bool bottomLeft = grid[12];

            if (clockwise)
            {
                grid[3] = topLeft;
                grid[15] = topRight;
                grid[12] = bottomRight;
                grid[0] = bottomLeft;
            }
            else
            {
                grid[12] = topLeft;
                grid[15] = bottomLeft;
                grid[3] = bottomRight;
                grid[0] = topRight;
            }
        }


        private void RotateGrid(bool[] grid, bool clockwise)
        {
            bool[] old = new bool[16];

            for (int i = 0; i < 16; i++)
                old[i] = grid[i];

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    int oldPosition = row * 4 + col;

                    if (clockwise)
                    {
                        int newRow = col;
                        int newCol = 3 - row;

                        grid[newRow * 4 + newCol] = old[oldPosition];
                    }
                    else
                    {
                        int newRow = 3 - col;
                        int newCol = row;

                        grid[newRow * 4 + newCol] = old[oldPosition];
                    }
                }
            }
        }
    }
}