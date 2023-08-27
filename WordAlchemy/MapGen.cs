
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

                    HeightMap[i * Cols + j] = remapNoise * MathHelper.FallOffMapCircular(j, i, Cols, Rows);
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
                terrain= Terrain.Sand;
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
}
