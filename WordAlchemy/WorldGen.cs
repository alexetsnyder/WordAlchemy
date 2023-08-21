﻿
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
            Noise.SetFractalType(FastNoiseLite.FractalType.FBm);

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

        public TerrainInfo GetTerrain(int x, int y)
        {
            float heightValue = HeightMap[y * Width + x];

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
        public static TerrainInfo Water = new TerrainInfo("~", 3, 8, Colors.Blue());

        public static TerrainInfo Grass = new TerrainInfo(",", 3, -2, Colors.Green());

        public static TerrainInfo Dirt = new TerrainInfo(".", 3, -3, Colors.Brown());

        public static TerrainInfo Hill = new TerrainInfo("\u0361", 0, 3, Colors.DarkGreen());

        public static TerrainInfo SmallHill = new TerrainInfo("\u032F", 2, -2, Colors.DarkGreen());

        public static TerrainInfo PointedHill = new TerrainInfo("\u02C4", 3, 0, Colors.DarkGreen());

        public static TerrainInfo Mountain = new TerrainInfo("\u0245", 3, -8, Colors.Grey());

        public static TerrainInfo SmallMountain = new TerrainInfo("\u028C", 3, -8, Colors.Grey());

        public static TerrainInfo SmallDoubleMountain = new TerrainInfo("\u028D", 3, -8, Colors.Grey());
    }

    public struct TerrainInfo
    {
        public string Symbol { get; set; }
        public SDL.SDL_Color Color { get; set; }
        public int CenterX { get; set; }
        public int CenterY { get; set; }

        public TerrainInfo(string symbol, int centerX, int centerY, SDL.SDL_Color color)
        {
            Symbol = symbol;
            CenterX = centerX;
            CenterY = centerY;
            Color = color;
        }
    }
}
