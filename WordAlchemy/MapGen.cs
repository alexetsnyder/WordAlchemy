
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
                    HeightMap[i * Cols + j] = Noise.GetNoise(i, j);
                }
            }
        }

        public TerrainInfo GetTerrain(int i, int j)
        {
            float heightValue = HeightMap[i * Cols + j];

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
}
