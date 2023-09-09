
namespace WordAlchemy.WorldGen
{
    public class Grid
    {
        public bool IsVisible { get; set; }

        public int CellWidth { get; set; }
        public int CellHeight { get; set; }

        public Grid(int cellWidth, int cellHeight)
        {
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public Cell GetCell(int i, int j)
        {
            return new Cell(j * CellWidth, i * CellHeight, CellWidth, CellHeight);
        }

        public Cell GetCellFromWorld(int worldX, int worldY)
        {
            int modX = (worldX < 0) ? (worldX % CellWidth) + CellWidth : worldX % CellWidth;
            int modY = (worldY < 0) ? (worldY % CellHeight) + CellHeight : worldY % CellHeight;

            int cellX = worldX - modX;
            int cellY = worldY - modY;

            return new Cell(cellX, cellY, CellWidth, CellHeight);
        }
    }

    public struct Cell
    {
        public int I { get; private set; }
        public int J { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Cell(int x, int y, int width, int height)
        {
            I = y / height;
            J = x / width;

            X = x;
            Y = y;

            Width = width;
            Height = height;
        }
    }
}
