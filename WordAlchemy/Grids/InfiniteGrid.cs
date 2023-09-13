namespace WordAlchemy.Grids
{
    public class InfiniteGrid
    {
        public bool IsVisible { get; set; }

        public Point GridOrigin { get; set; }

        public Point CellSize { get; set; }

        public InfiniteGrid(Point gridOrigin, Point cellSize)
        {
            IsVisible = false;

            GridOrigin = gridOrigin;
            CellSize = cellSize;
        }

        public virtual Cell? GetCell(Point gridPos)
        {
            return new Cell(gridPos, CellSize, true);
        }

        public virtual Cell? GetCellFromWorld(Point worldPos)
        {
            Point worldGridPos = new Point(worldPos) - GridOrigin;

            int modX = worldGridPos.X < 0 ? worldGridPos.X % CellSize.W + CellSize.W : worldGridPos.X % CellSize.W;
            int modY = worldGridPos.Y < 0 ? worldGridPos.Y % CellSize.H + CellSize.H : worldGridPos.Y % CellSize.H;

            worldGridPos -= new Point(modX, modY);

            return new Cell(worldGridPos, CellSize, false);
        }
    }
}
