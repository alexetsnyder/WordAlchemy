
using SDL2;

namespace WordAlchemy.Tools
{
    internal class FontViewer
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public TerrainInfo CurrentTerrain { get; set; }

        private SDLGraphics Graphics { get; set; }

        public FontViewer(TerrainInfo terrainInfo)
        {
            CurrentTerrain = terrainInfo;

            Graphics = SDLGraphics.Instance;
            Graphics.SizeText(CurrentTerrain.Symbol, FontName.UNIFONT, out int width, out int height);

            Width = width;
            Height = height;
        }

        public void Draw(SDLGraphics graphics)
        {
            int centerX = AppSettings.Instance.WindowWidth / 2;
            int centerY = AppSettings.Instance.WindowHeight / 2;

            SDL.SDL_Rect rect = new SDL.SDL_Rect
            {
                x = centerX,
                y = centerY,
                w = 10,
                h = 10,
            };

            graphics.SetDrawColor(Colors.White());

            graphics.DrawRect(ref rect);


            int centerXMod = 0;
            int centerYMod = 0; 

            Graphics.DrawText(CurrentTerrain.Symbol, centerX + centerXMod, centerY + centerYMod, Colors.Red(), FontName.UNIFONT);
        }
    }
}
