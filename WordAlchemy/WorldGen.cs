
using SDL2;

namespace WordAlchemy
{
    internal class WorldGen
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
      
        public int Seed { get; set; }

        private FastNoiseLite Noise { get; set; }
        private float[] HeightMap { get; set; }

        public WorldGen(int rows, int cols, int seed = 0)
        {
            Rows = rows;
            Cols = cols;
                 
            Seed = seed;
            Noise = new FastNoiseLite(seed);
            Noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            Noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            HeightMap = new float[Cols * Rows];
        }

        public void GenerateHeightMap()
        {
            int index = 0;
            
            for (int x = 0; x < Cols; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    HeightMap[index++] = Noise.GetNoise(x, y);
                }
            }
        }

        public TerrainInfo GetTerrain(int x, int y)
        {
            float heightValue = HeightMap[x * Rows + y];

            TerrainInfo terrain;
            if (heightValue < -0.25f)
            {
                terrain = Terrain.Water;
            }
            else if (heightValue < 0.0f)
            {
                terrain = Terrain.Grass;
            }
            else if (heightValue < 0.25f)
            {
                terrain = Terrain.SmallHill;
            }
            else //(heightValue < 0.5f)
            {
                terrain = Terrain.SmallMountain;
            }

            return terrain;
        }
    }

    public static class Terrain
    {
        public static TerrainInfo Water = new TerrainInfo("~", 0, 0, Colors.Blue());

        public static TerrainInfo Grass = new TerrainInfo("n", 0,  -2, Colors.Green());

        public static TerrainInfo Dirt = new TerrainInfo(".", 8,  -2, Colors.Brown());

        public static TerrainInfo Hill = new TerrainInfo("\u0361", -2, 8, Colors.DarkGreen());

        public static TerrainInfo SmallHill = new TerrainInfo("\u032F", 8, -2, Colors.DarkGreen());

        public static TerrainInfo PointedHill = new TerrainInfo("\u02C4", 0, 0, Colors.DarkGreen());

        public static TerrainInfo Mountain = new TerrainInfo("\u0245", 8, -4, Colors.Grey());

        public static TerrainInfo SmallMountain = new TerrainInfo("\u028C", 8, -4, Colors.Grey());

        public static TerrainInfo SmallDoubleMountain = new TerrainInfo("\u028D", 0, -4, Colors.Grey());
    }

    public struct TerrainInfo
    {
        public string Symbol { get; set; }
        public SDL.SDL_Color Color { get; set; }
        public int WidthDivisor { get; set; }
        public int HeightDivisor { get; set; }

        public TerrainInfo(string symbol, int centerX, int centerY, SDL.SDL_Color color)
        {
            Symbol = symbol;
            WidthDivisor = centerX;
            HeightDivisor = centerY;
            Color = color;
        }
    }
}
