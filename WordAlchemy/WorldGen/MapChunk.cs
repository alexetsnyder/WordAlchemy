
using SDL2;
using WordAlchemy.Settings;
using WordAlchemy.Systems;

namespace WordAlchemy.WorldGen
{
    public class MapChunk
    {
        public int X {  get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int Rows { get; set; }
        public int Cols { get; set; }

        public Grid Grid { get; set; }

        public byte[] GridCells { get; set; }

        private IntPtr ChunkTexture { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public MapChunk(int x, int y, int rows, int cols, int width, int height)
        {
            X = x;
            Y = y;

            Width = width;
            Height = height;

            Rows = rows;
            Cols = cols;

            Grid = new Grid(width / cols, height / rows);
            GridCells = new byte[rows * cols];

            ChunkTexture = IntPtr.Zero;

            GraphicSystem = GraphicSystem.Instance;  
        }

        public void GenerateChunkTexture()
        {
            IntPtr chunkSurface = GenerateChunkSurface();

            ChunkTexture = GraphicSystem.CreateTextureFromSurface(chunkSurface);

            SDL.SDL_FreeSurface(chunkSurface);
        }

        public IEnumerable<Tuple<byte, Cell>> GetCells()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    yield return new Tuple<byte, Cell>(GridCells[i * Cols + j], Grid.GetCell(i, j));
                }
            }
        }

        private IntPtr GenerateChunkSurface()
        {
            IntPtr chunkSurface = GraphicSystem.CreateSurface(Width, Height);

            foreach (var byteCellTuple in GetCells())
            {
                DrawTerrain(chunkSurface, byteCellTuple.Item1, byteCellTuple.Item2);
            }

            return chunkSurface;
        }

        private void DrawTerrain(IntPtr chunkSurface, byte terrainByte, Cell cell)
        {
            TerrainInfo terrainInfo = Terrain.TerrainArray[terrainByte];

            int x = cell.X + terrainInfo.XMod;
            int y = cell.Y + terrainInfo.YMod;

            GraphicSystem.BlitText(chunkSurface, terrainInfo.Symbol, x, y, terrainInfo.Color, AppSettings.Instance.MapFontName);
        }

        public void Draw(ref SDL.SDL_Rect src, ref SDL.SDL_Rect dst)
        {
            GraphicSystem.DrawTexture(ChunkTexture, ref src, ref dst);
        }
    }
}
