using System;

public partial class NotRoundKeypadScript
{
    public class Position : IEquatable<Position>
    {
        public int X;
        public int Y;
        public int Z;

        public Position(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Position GetThirdToFormSet(Position a, Position b)
        {
            int cx = (6 - a.X - b.X) % 3;
            int cy = (6 - a.Y - b.Y) % 3;
            int cz = (6 - a.Z - b.Z) % 3;
            return new Position(cx, cy, cz);
        }

        public bool Equals(Position other)
        {
            return other != null && other.X == X && other.Y == Y && other.Z == Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Position && Equals((Position)obj);
        }

        public override int GetHashCode()
        {
            return X + Y * 3 + Z * 9;
        }
    }
}
