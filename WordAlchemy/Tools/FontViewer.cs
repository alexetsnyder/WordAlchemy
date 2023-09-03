﻿
using SDL2;
using WordAlchemy.WorldGen;

namespace WordAlchemy.Tools
{
    internal class FontViewer
    {
        public List<TerrainInfo> TerrainInfoList = new List<TerrainInfo>();

        public int Width { get; set; }
        public int Height { get; set; }


        private SDLGraphics Graphics { get; set; }

        public FontViewer()
        {
            TerrainInfoList.Add(Terrain.Water);
            TerrainInfoList.Add(Terrain.Sand);
            TerrainInfoList.Add(Terrain.Grass);
            TerrainInfoList.Add(Terrain.Dirt);
            TerrainInfoList.Add(Terrain.Hill);
            TerrainInfoList.Add(Terrain.Mountain);

            Graphics = SDLGraphics.Instance;
            Graphics.SizeText(Terrain.Water.Symbol, AppSettings.Instance.MapFontName, out int width, out int height);

            Width = width;
            Height = height;
        }

        public void Draw(SDLGraphics graphics)
        {
            int viewerWidth = TerrainInfoList.Count * 2 * Width;
            int ViewerHeight = TerrainInfoList.Count * 2 * Height;

            int centerX = AppSettings.Instance.WindowWidth / 2;
            int centerY = AppSettings.Instance.WindowHeight / 2;

            int startX = centerX - viewerWidth / 2;
            int startY = centerY - ViewerHeight / 2;

            foreach (var terrain in TerrainInfoList)
            {
                SDL.SDL_Rect rect = new SDL.SDL_Rect
                {
                    x = startX,
                    y = startY,
                    w = Width,
                    h = Height,
                };

                graphics.SetDrawColor(Colors.White());

                graphics.DrawRect(ref rect);

                Graphics.DrawText(terrain.Symbol, startX + terrain.XMod, startY + terrain.YMod, terrain.Color, AppSettings.Instance.MapFontName);

                startX +=  2 * Width;
            }
        }
    }
}
