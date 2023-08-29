
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
            //GenerateRivers();
        }

        private void GenerateHeightMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    float noise = Noise.GetNoise(i, j);
                    float remapNoise = MathHelper.Remap(noise, -1.0f, 1.0f, 0.0f, 100.0f);
                    float fallOffValue = MathHelper.SigmoidFallOffMapCircular(j, i, Cols, Rows);

                    HeightMap[i * Cols + j] = remapNoise * fallOffValue;
                }
            }
        }

        private void GenerateRivers()
        {
            int[] riverStarts = GetMaxHeights(6);
            
            foreach (int index in riverStarts)
            {
                int j = index % Cols;
                int i = (index - j) / Cols;

                if (j > Cols / 2)
                {
                    j++;
                    if (j < Cols)
                    {
                        int newIndex = i * Cols + j;
                        float height = HeightMap[newIndex];

                        while (height >= 20.0f && j < Cols)
                        {
                            if (height < 60.0f)
                            {
                                HeightMap[newIndex] = 19.0f;
                            }                 

                            j++;
                            newIndex = i * Cols + j;
                            height = HeightMap[newIndex];
                        }
                    }            
                }
                else
                {
                    j--;
                    if (j >= 0)
                    {
                        int newIndex = i * Cols + j;
                        float height = HeightMap[newIndex];

                        while (height >= 20.0f && j >= 0)
                        {
                            if (height < 60.0f)
                            {
                                HeightMap[newIndex] = 19.0f;
                            }

                            j--;
                            newIndex = i * Cols + j;
                            height = HeightMap[newIndex];
                        }
                    }
                }
            }
        }

        private int[] GetMaxHeights(int n = 1)
        {
            float[] maxValues = new float[n];
            int[] maxIndexes = new int[n];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    float heightValue = HeightMap[i * Cols + j];

                    for (int k = 0; k < n; k++)
                    {
                        if (heightValue > maxValues[k])
                        {
                            maxValues[k] = heightValue;
                            maxIndexes[k] = i * Cols + j;
                            break;
                        }
                    }
                }
            }

            return maxIndexes;
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
                terrain = Terrain.Hill;
            }
            else //(heightValue < 100.0f)
            {
                terrain = Terrain.Mountain;
            }

            return terrain;
        }
    }

    public static class Terrain
    {
        public static TerrainInfo Water = new TerrainInfo("~", Colors.Blue());

        public static TerrainInfo Sand = new TerrainInfo("~", Colors.Sand());

        public static TerrainInfo Grass = new TerrainInfo(",", Colors.Green());

        public static TerrainInfo Dirt = new TerrainInfo(".", Colors.Brown());

        public static TerrainInfo Hill = new TerrainInfo("∩", Colors.DarkGreen());

        public static TerrainInfo Mountain = new TerrainInfo("▲", Colors.Grey());
    }

    public struct TerrainInfo
    {
        public string Symbol { get; set; }
        public SDL.SDL_Color Color { get; set; }

        public TerrainInfo(string symbol, SDL.SDL_Color color)
        {
            Symbol = symbol;
            Color = color;
        }
    }
}
