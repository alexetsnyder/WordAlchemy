
using WordAlchemy.WorldGen;

namespace WordAlchemy.Grids
{
    public class BoundedGrid : InfiniteGrid
    {
        public int Rows { get; set; }
        public int Cols { get; set; }

        public Point Size { get; set; }

        private byte[] GridCellValues { get; set; }

        public BoundedGrid(Point gridOrigin, Point gridRowsCols, Point cellSize) : base(gridOrigin, cellSize)
        {
            Rows = gridRowsCols.X;
            Cols = gridRowsCols.Y;

            Size = new Point(Cols * cellSize.W, Rows * cellSize.H);
            GridCellValues = new byte[Rows * Cols];
        }

        public IEnumerable<Cell> GetCells()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    yield return new Cell(new Point(i, j), CellSize, true);
                }
            }
        }

        public int GetIndex(Cell cell)
        {
            return cell.GridPos.I * Cols + cell.GridPos.J; 
        }

        public byte GetCellValue(Cell cell)
        {
            return GridCellValues[GetIndex(cell)];
        }

        public void SetCellValue(Cell cell, byte value)
        {
            GridCellValues[GetIndex(cell)] = value;
        }

        public override Cell? GetCell(Point gridPos)
        {
            if (IsInGrid(gridPos))
            {
                return base.GetCell(gridPos);
            }

            return null;
        }

        public override Cell? GetCellFromWorld(Point worldPos)
        {
            if (worldPos.X >= 0 && worldPos.X < Size.W &&
                worldPos.Y >= 0 && worldPos.Y < Size.H)
            {
                return base.GetCellFromWorld(worldPos);
            }

            return null;   
        }

        public bool IsInGrid(Point gridPos)
        {
            if (gridPos.I >= 0 && gridPos.I < Rows &&
                gridPos.J >= 0 && gridPos.J < Cols)
            {
                return true;
            }

            return false;
        }

        public List<Cell> GetConnectedCells(Cell cell)
        {
            List<Point> connectedCellIndexes = new List<Point>()
            {
                new Point(cell.GridPos.I - 1, cell.GridPos.J - 1),
                new Point(cell.GridPos.I - 1, cell.GridPos.J    ),
                new Point(cell.GridPos.I - 1, cell.GridPos.J + 1),
                new Point(cell.GridPos.I,     cell.GridPos.J - 1),
                new Point(cell.GridPos.I,     cell.GridPos.J + 1),
                new Point(cell.GridPos.I + 1, cell.GridPos.J - 1),
                new Point(cell.GridPos.I + 1, cell.GridPos.J    ),
                new Point(cell.GridPos.I + 1, cell.GridPos.J + 1),
            };

            return connectedCellIndexes.Where(p => IsInGrid(p)).Select(p => new Cell(p, CellSize, true)).ToList();
        }
    }
}
