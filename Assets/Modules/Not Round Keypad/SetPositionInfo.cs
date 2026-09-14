using System;

public partial class NotRoundKeypadScript
{
    public class SetPositionInfo : IEquatable<SetPositionInfo>
    {
        public int X;
        public int Y;
        public int Z;

        public SetPositionInfo(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static SetPositionInfo GetThirdToFormSet(SetPositionInfo a, SetPositionInfo b)
        {
            int cx = (6 - a.X - b.X) % 3;
            int cy = (6 - a.Y - b.Y) % 3;
            int cz = (6 - a.Z - b.Z) % 3;
            return new SetPositionInfo(cx, cy, cz);
        }

        public bool Equals(SetPositionInfo other)
        {
            return other != null && other.X == X && other.Y == Y && other.Z == Z;
        }

        public override bool Equals(object obj)
        {
            return obj is SetPositionInfo && Equals((SetPositionInfo)obj);
        }

        public override int GetHashCode()
        {
            return X + Y * 3 + Z * 9;
        }

        public bool SharesTwoAxes(SetPositionInfo other)
        {
            int matches = 0;
            if (other.X == X)
                matches++;
            if (other.Y == Y)
                matches++;
            if (other.Z == Z)
                matches++;
            return matches >= 2;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }
    }
}
