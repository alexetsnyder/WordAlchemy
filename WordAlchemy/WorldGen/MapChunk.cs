
using SDL2;
using WordAlchemy.Grids;
using WordAlchemy.Settings;
using WordAlchemy.Systems;

namespace WordAlchemy.WorldGen
{
    public class MapChunk
    {
        public Point ChunkPos { get; set; }

        public Point ChunkSize { get; set; }

        public Cell MapCell { get; set; }

        public BoundedGrid Grid { get; set; }

        private IntPtr ChunkTexture { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public MapChunk(Cell cell, Point chunkPos, int rows, int cols, Point chunkSize)
        {
            ChunkPos = chunkPos;
            ChunkSize = chunkSize;

            MapCell = cell;
            Grid = new BoundedGrid(new Point(0, 0), new Point(rows, cols), new Point(chunkSize.W / cols, chunkSize.H / rows));

            ChunkTexture = IntPtr.Zero;

            GraphicSystem = GraphicSystem.Instance;  
        }

        public void GenerateChunkTexture()
        {
            IntPtr chunkSurface = GenerateChunkSurface();

            ChunkTexture = GraphicSystem.CreateTextureFromSurface(chunkSurface);

            SDL.SDL_FreeSurface(chunkSurface);
        }

        private IntPtr GenerateChunkSurface()
        {
            IntPtr chunkSurface = GraphicSystem.CreateSurface(ChunkSize.W, ChunkSize.H);

            foreach (Cell cell in Grid.GetCells())
            {
                DrawTerrain(chunkSurface, cell, Grid.GetCellValue(cell));
            }

            return chunkSurface;
        }

        private void DrawTerrain(IntPtr chunkSurface, Cell cell, byte terrainByte)
        {
            TerrainInfo terrainInfo = Terrain.TerrainArray[terrainByte];

            int x = cell.WorldPos.X + terrainInfo.XMod;
            int y = cell.WorldPos.Y + terrainInfo.YMod;

            GraphicSystem.BlitText(chunkSurface, terrainInfo.Symbol, x, y, terrainInfo.Color, AppSettings.Instance.MapFontName);
        }

        public void Draw(ref SDL.SDL_Rect src, ref SDL.SDL_Rect dst)
        {
            GraphicSystem.DrawTexture(ChunkTexture, ref src, ref dst);
        }
    }
}
