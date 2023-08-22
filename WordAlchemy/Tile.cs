
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
            Graphics.SizeText(Info.Symbol, "unifont", out int width, out int height);

            Width = width;
            Height = height;
        }

        public void Draw(Grid grid)
        {
            grid.CellToScreen(GridX, GridY, out int screenX, out int screenY);
            int centerX = screenX;
            int centerY = screenY;

            int centerXMod = (Info.CenterX == 0) ? 0 : Width / Info.CenterX;
            int centerYMod = (Info.CenterY == 0) ? 0 : Height / Info.CenterY;

            Graphics.DrawText(Info.Symbol, centerX + centerXMod, centerY + centerYMod, Info.Color, "unifont");
        }
    }
}
