
namespace WordAlchemy.Grids
{
    public readonly struct Cell
    {
        public Point GridPos { get; }

        public Point WorldPos { get; }

        public Point CellSize { get; }

        public Cell(Point pos, Point cellSize, bool isGridPos)
        {
            CellSize = cellSize;
            if (isGridPos)
            {
                GridPos = pos;
                WorldPos = new Point(pos.J * cellSize.W, pos.I * cellSize.H);
            }
            else
            {
                WorldPos = pos;
                GridPos = new Point(pos.Y / cellSize.H, pos.X / cellSize.W);
            } 
        }
    }
}