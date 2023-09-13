
using SDL2;
using WordAlchemy.Grids;
using WordAlchemy.Settings;
using WordAlchemy.Systems;

namespace WordAlchemy.WorldGen
{
    public class Map
    {
        public BoundedGrid Grid { get; set; }

        public List<Group> GroupList { get; set; }

        public MapGen MapGen { get; set; }

        public Cell? SelectedCell { get; set; }

        private IntPtr MapTexture { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public Map(MapGen mapGen)
        {
            MapGen = mapGen;

            Grid = new BoundedGrid(new Point(0, 0), new Point(MapGen.Rows, MapGen.Cols), MapGen.CharSize);

            GroupList = new List<Group>();

            SelectedCell = null;

            MapTexture = IntPtr.Zero;

            GraphicSystem = GraphicSystem.Instance;
        }

        public void GenerateMapTexture()
        {
            MapTexture = GraphicSystem.CreateTexture(MapGen.Width, MapGen.Height);

            foreach (Cell cell in Grid.GetCells())
            {
                DrawTerrain(cell, Grid.GetCellValue(cell));
            }
        }

        public bool IsCellGrouped(Cell cell)
        {
            foreach (Group group in GroupList)
            {
                if (group.IsCellInGroup(cell))
                {
                    return true;
                }
            }
            return false;
        }

        private void DrawTerrain(Cell cell, byte terrainByte)
        {
            TerrainInfo terrainInfo = Terrain.TerrainArray[terrainByte];

            int x = cell.WorldPos.X + terrainInfo.XMod;
            int y = cell.WorldPos.Y + terrainInfo.YMod;

            GraphicSystem.DrawTextToTexture(MapTexture, terrainInfo.Symbol, x, y, terrainInfo.Color, AppSettings.Instance.MapFontName);
        }

        public void Draw(ref SDL.SDL_Rect src,  ref SDL.SDL_Rect dest)
        {
            GraphicSystem.DrawTexture(MapTexture, ref src, ref dest);
        }

        public Group? GetGroup(Cell cell)
        {
            foreach (Group group in GroupList)
            {
                if (group.IsCellInGroup(cell))
                {
                    return group;
                }
            }

            return null;
        }

        public Cell? GetCell(Point worldPos)
        {
            return Grid.GetCellFromWorld(worldPos);
        }
    }
}
