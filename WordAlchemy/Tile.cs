
namespace WordAlchemy
{
    internal class Tile
    {
        public int GridX {  get; set; }
        public int GridY { get; set; }

        public Text TileText { get; set; }

        public TerrainInfo Info { get; set; }

        public Tile(Text tileText, TerrainInfo terrainInfo, int gridX, int gridY)
        {
            TileText = tileText;
            Info = terrainInfo;

            GridX = gridX;
            GridY = gridY;
        }

        public void Draw(Grid grid)
        {
            grid.CellToScreen(GridX, GridY, out int screenX, out int screenY);
            int centerX = screenX;
            int centerY = screenY;

            int centerXMod = (Info.CenterX == 0) ? 0 : TileText.Width / Info.CenterX;
            int centerYMod = (Info.CenterY == 0) ? 0 : TileText.Height / Info.CenterY;

            TileText.Draw(centerX + centerXMod, centerY + centerYMod);
        }
    }
}
