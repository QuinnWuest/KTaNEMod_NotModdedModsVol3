using System;
using System.Diagnostics;

public partial class NotRoundKeypadScript
{
    public class TunnelPosition : IEquatable<TunnelPosition>
    {
        public int? X;
        public int? Y;
        public int? Z;
        public int? FacingWall;
        public int? UpWall;
        public int? RightWall;
        public char KeypadSymbol;

        public TunnelPosition(int? x = null, int? y = null, int? z = null, int? facingWall = null, int? upWall = null, int? rightWall = null, char kps = ' ')
        {
            X = x;
            Y = y;
            Z = z;
            FacingWall = facingWall;
            UpWall = upWall;
            RightWall = rightWall;
            KeypadSymbol = kps;
        }

        public bool Equals(TunnelPosition other)
        {
            return other != null && other.X == X && other.Y == Y && other.Z == Z;
        }

        public override bool Equals(object obj)
        {
            return obj is TunnelPosition && Equals((TunnelPosition)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
                hash = hash * 31 + Z.GetHashCode();
                return hash;
            }
        }

        public static TunnelPosition ApplyMovement(TunnelPosition position, TunnelDirection direction)
        {
            int? temp;
            var p = new TunnelPosition(position.X, position.Y, position.Z, position.FacingWall, position.UpWall, position.RightWall, position.KeypadSymbol);
            if (direction == TunnelDirection.Up)
            {
                temp = p.UpWall;
                p.UpWall = 7 - p.FacingWall;
                p.FacingWall = temp;
            }
            else if (direction == TunnelDirection.Down)
            {
                temp = p.FacingWall;
                p.FacingWall = 7 - p.UpWall;
                p.UpWall = temp;
            }
            else if (direction == TunnelDirection.Left)
            {
                temp = p.FacingWall;
                p.FacingWall = 7 - p.RightWall;
                p.RightWall = temp;
            }
            else if (direction == TunnelDirection.Right)
            {
                temp = p.RightWall;
                p.RightWall = 7 - p.FacingWall;
                p.FacingWall = temp;
            }
            else if (direction == TunnelDirection.Clockwise) // only used for initial positioning
            {
                temp = p.RightWall;
                p.RightWall = 7 - p.UpWall;
                p.UpWall = temp;
            }

            if (direction != TunnelDirection.Clockwise)
            {
                if (p.FacingWall == 1)
                    p.Z--;
                else if (p.FacingWall == 2)
                    p.Y--;
                else if (p.FacingWall == 3)
                    p.X--;
                else if (p.FacingWall == 4)
                    p.X++;
                else if (p.FacingWall == 5)
                    p.Y++;
                else if (p.FacingWall == 6)
                    p.Z++;
            }
            return p;
        }

        public bool IsValidTunnelPosition()
        {
            if (X < 0 || X > 2 || Y < 0 || Y > 2 || Z < 0 || Z > 2)
                return false;
            return true;
        }

        public string ToStringCurrent()
        {
            return string.Format("({0}, {1}, {2}). Keypad symbol is {3}. Facing wall {4}. Above wall is wall {5}", X, Y, Z, KeypadSymbol, FacingWall, UpWall);
        }

        public string ToStringGoal()
        {
            return string.Format("({0}, {1}, {2}). Keypad symbol is {3}.", X, Y, Z, KeypadSymbol);
        }
    }
}
