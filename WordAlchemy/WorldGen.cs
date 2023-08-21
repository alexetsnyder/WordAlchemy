
using SDL2;

namespace WordAlchemy
{
    internal class WorldGen
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int Seed { get; set; }

        private FastNoiseLite Noise { get; set; }
        private float[] HeightMap { get; set; }

        public WorldGen(int width, int height, int seed = 0)
        {
            Width = width;
            Height = height;
            
            Seed = seed;
            Noise = new FastNoiseLite(seed);
            Noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

            HeightMap = new float[Width * Height];
        }

        public void GenerateHeightMap()
        {
            int index = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    HeightMap[index++] = Noise.GetNoise(x, y);
                }
            }
        }

        public string GetTerrainType(int x, int y)
        {
            float heightValue = HeightMap[y * Width + x];

            if (heightValue < 0)
            {
                return ".";
            }
            else
            {
                return "\u028D";
            }
        }

        public Terrain GetTerrain(int x, int y)
        {
            float heightValue = HeightMap[y * Width + x];

            Terrain terrain;
            if (heightValue < -0.25f)
            {
                terrain = new Terrain(Terrain.Water, Colors.Blue());
            }
            else if (heightValue < 0.0f)
            {
                terrain = new Terrain(Terrain.Grass, Colors.Green());
            }
            else if (heightValue < 0.25f)
            {
                terrain = new Terrain(Terrain.SmallHill, Colors.DarkGreen());
            }
            else //(heightValue < 0.5f)
            {
                terrain = new Terrain(Terrain.SmallMountain, Colors.Grey());
            }

            return terrain;
        }
    }

    public class Terrain
    {
        public string Symbol { get; set; }
        public SDL.SDL_Color Color { get; set; }

        public Terrain(string symbol, SDL.SDL_Color color)
        {
            Symbol = symbol;
            Color = color;
        }

        public static string Water = "~";

        public static string Grass = ",";

        public static string Dirt = ".";

        public static string Hill = "\u0361";

        public static string SmallHill = "\u032F";

        public static string PointedHill = "\u02C4";

        public static string Mountain = "\u0245";

        public static string SmallMountain = "\u028C";

        public static string SmallDoubleMountain = "\u028D";

    }
}
