
using SDL2;

namespace WordAlchemy.Tools
{
    internal class FontViewer
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public TerrainInfo CurrentTerrain { get; set; }

        private SDLGraphics Graphics { get; set; }

        public FontViewer(TerrainInfo terrainInfo, int windowWidth, int windowHeight)
        {
            CurrentTerrain = terrainInfo;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            Graphics = SDLGraphics.Instance;
            Graphics.SizeText(CurrentTerrain.Symbol, "unifont", out int width, out int height);

            Width = width;
            Height = height;

            WireEvents();
        }

        private void WireEvents()
        {
            EventSystem eventSystem = EventSystem.Instance;
            eventSystem.Listen(SDL.SDL_EventType.SDL_KEYDOWN, KeyDownEvent);
        }

        public void Draw(SDLGraphics graphics)
        {
            int centerX = WindowWidth / 2;
            int centerY = WindowHeight / 2;

            SDL.SDL_Rect rect = new SDL.SDL_Rect
            {
                x = centerX,
                y = centerY,
                w = 10,
                h = 10,
            };

            graphics.SetDrawColor(Colors.White());

            graphics.DrawRect(ref rect);


            int centerXMod = (CurrentTerrain.WidthDivisor == 0) ? 0 : Width / CurrentTerrain.WidthDivisor;
            int centerYMod = (CurrentTerrain.HeightDivisor == 0) ? 0 : Height / CurrentTerrain.HeightDivisor;

            Graphics.DrawText(CurrentTerrain.Symbol, centerX + centerXMod, centerY + centerYMod, Colors.Red(), "unifont");
        }

        public void KeyDownEvent(SDL.SDL_Event e)
        {
            
        }
    }
}
