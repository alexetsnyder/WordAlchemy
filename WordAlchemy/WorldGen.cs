
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
}
