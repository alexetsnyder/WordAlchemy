
namespace WordAlchemy.Grids
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int I
        {
            get
            {
                return X;
            }
            set
            {
                X = value;
            }
        }
        public int J
        {
            get
            {
                return Y;
            }
            set
            {
                Y = value;
            }
        }

        public int W
        {
            get
            {
                return X;
            }
            set
            {
                X = value;
            }
        }
        public int H
        {
            get
            {
                return Y;
            }
            set
            {
                Y = value;
            }
        }

        public Tuple<int, int> PointTuple
        {
            get
            {
                return Tuple.Create(X, Y);
            }
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(Tuple<int, int> tuple)
        {
            X = tuple.Item1;
            Y = tuple.Item2;
        }

        public Point(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
    }
}
