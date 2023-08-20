
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
    }
}
