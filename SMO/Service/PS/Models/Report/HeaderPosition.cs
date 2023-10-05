using System.Collections.Generic;

namespace SMO.Service.PS.Models.Report
{
    public class HeaderPosition : IEqualityComparer<HeaderPosition>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool Equals(HeaderPosition x, HeaderPosition y)
        {
            return x.X == y.X && x.Y == y.Y;
        }

        public int GetHashCode(HeaderPosition obj)
        {
            return obj.X.GetHashCode() ^ obj.Y.GetHashCode();
        }
    }
}
