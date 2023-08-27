
using SDL2;
using WordAlchemy.Helpers;

namespace WordAlchemy
{
    internal class MapGen
    {
        public int Rows { get; set; }
        public int Cols { get; set; }

        public int Seed { get; set; }

        private FastNoiseLite Noise { get; set; }
        private float[] HeightMap { get; set; }

        public MapGen(int rows, int cols, int seed)
        {
            Rows = rows;
            Cols = cols;

            Seed = seed;
            Noise = new FastNoiseLite(seed);
            Noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            Noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            HeightMap = new float[Cols * Rows];
        }

        public void GenerateMap()
        {
            GenerateHeightMap();
        }

        private void GenerateHeightMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    float noise = Noise.GetNoise(i, j);
                    float remapNoise = MathHelper.Remap(noise, -1.0f, 1.0f, 0.0f, 100.0f);

                    HeightMap[i * Cols + j] = remapNoise * MathHelper.SigmoidFallOffMapCircular(j, i, Cols, Rows);
                }
            }
        }

        public TerrainInfo GetTerrain(int i, int j)
        {
            float heightValue = HeightMap[i * Cols + j];

            TerrainInfo terrain;
            if (heightValue < 20.0f)
            {
                terrain = Terrain.Water;
            }
            else if (heightValue < 25.0f)
            {
                terrain = Terrain.Sand;
            }
            else if (heightValue < 40.0f)
            {
                terrain = Terrain.Grass;
            }
            else if (heightValue < 60.0f)
            {
                terrain = Terrain.SmallHill;
            }
            else //(heightValue < 100.0f)
            {
                terrain = Terrain.SmallMountain;
            }

            return terrain;
        }
    }

    public static class Terrain
    {
        public static TerrainInfo Water = new TerrainInfo("~", 0, 0, Colors.Blue());

        public static TerrainInfo Sand = new TerrainInfo("~", 0, 0, Colors.Sand());

        public static TerrainInfo Grass = new TerrainInfo(",", 0, -2, Colors.Green());

        public static TerrainInfo Dirt = new TerrainInfo(".", 8, -2, Colors.Brown());

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
