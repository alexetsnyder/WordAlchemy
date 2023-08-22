
using SDL2;

namespace WordAlchemy.Tools
{
    internal class FontViewer
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public TerrainInfo CurrentTerrain { get; set; }

        public Text Symbol { get; set; }

        public FontViewer(TerrainInfo terrainInfo, int windowWidth, int windowHeight)
        {
            CurrentTerrain = terrainInfo;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            Font font = new Font("Assets/Fonts/unifont.ttf", 24);
            Symbol = new Text(font, CurrentTerrain.Symbol, CurrentTerrain.Color);

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
                w = 20,
                h = 20,
            };

            graphics.SetDrawColor(Colors.Black());

            graphics.DrawRect(ref rect);

            Symbol.CreateTexture();

            int centerXMod = (CurrentTerrain.CenterX == 0) ? 0 : Symbol.Width / CurrentTerrain.CenterX;
            int centerYMod = (CurrentTerrain.CenterY == 0) ? 0 : Symbol.Height / CurrentTerrain.CenterY;

            Symbol.Draw(centerX + centerXMod, centerY + centerYMod);
        }

        public void KeyDownEvent(SDL.SDL_Event e)
        {
            
        }
    }
}
