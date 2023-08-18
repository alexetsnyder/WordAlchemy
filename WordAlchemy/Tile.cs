
namespace WordAlchemy
{
    internal class Tile
    {
        public int GridX {  get; set; }
        public int GridY { get; set; }

        public Text TileText { get; set; }

        public Tile(Text tileText, int gridX, int gridY)
        {
            TileText = tileText;

            GridX = gridX;
            GridY = gridY;
        }

        public void Draw(SDLGraphics graphics, Grid grid)
        {
            grid.CellToScreen(GridX, GridY, out int screenX, out int screenY);
            int centerX = screenX - grid.CellWidth / 2;
            int centerY = screenY - grid.CellWidth / 2;

            TileText.Draw(graphics, centerX - TileText.Width / 2, centerY - TileText.Height / 2);
        }
    }
}
