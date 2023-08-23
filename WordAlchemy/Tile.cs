
using SDL2;

namespace WordAlchemy
{
    internal class Tile
    {
        public int GridX {  get; set; }
        public int GridY { get; set; }

        private int Width { get; set; }
        private int Height { get; set; }

        public TerrainInfo Info { get; set; }

        private SDLGraphics Graphics { get; set; }

        public Tile(TerrainInfo terrainInfo, int gridX, int gridY)
        {
            Info = terrainInfo;

            GridX = gridX;
            GridY = gridY;

            Graphics = SDLGraphics.Instance;
            Graphics.SizeText(Info.Symbol, FontName.UNIFONT, out int width, out int height);

            Width = width;
            Height = height;
        }

        public void Draw(Grid grid)
        {
            grid.CellToScreen(GridX, GridY, out int screenX, out int screenY);
            int centerX = screenX;
            int centerY = screenY;

            int centerXMod = (Info.WidthDivisor == 0) ? 0 : Width / Info.WidthDivisor;
            int centerYMod = (Info.HeightDivisor == 0) ? 0 : Height / Info.HeightDivisor;

            Graphics.DrawText(Info.Symbol, centerX + centerXMod, centerY + centerYMod, Info.Color, FontName.UNIFONT);
        }
    }
}
