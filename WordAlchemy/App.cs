using SDL2;

namespace WordAlchemy
{
    internal class App
    {
        public bool IsRunning { get; set; }

        private SDLGraphics Graphics { get; set; }

        private Grid GGrid { get; set; }

        private Text GameText { get; set; }

        public App()
        {
            IsRunning = true;
            Graphics = new SDLGraphics();
            
            if (!Graphics.Init())
            {
                IsRunning = false;
            }

            if (SDL_ttf.TTF_Init() < 0)
            {
                System.Diagnostics.Debug.WriteLine($"Couldn't initialize SDL TTF: {SDL.SDL_GetError()}");
                IsRunning = false;
            }
            else
            {
                CreateText();
            }

            GGrid = new Grid(1040, 880);
        }

        public void Run()
        {
            while (IsRunning)
            {
                PollEvents();

                Render();
            }

            Graphics.CleanUp();
            SDL_ttf.TTF_Quit();
        }

        private void PollEvents()
        {
            foreach (var e in Graphics.PollEvents())
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        IsRunning = false;
                        break;
                    case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        System.Diagnostics.Debug.WriteLine($"{e.button.button}");

                        SDL.SDL_GetMouseState(out int x, out int y);
                        System.Diagnostics.Debug.WriteLine($"Mouse X: {x}, Mouse Y: {y}");

                        GGrid.ScreenToCell(x, y, out int cellX, out int cellY);
                        System.Diagnostics.Debug.WriteLine($"Cell X: {cellX}, Cell Y: {cellY}");

                        GGrid.CellToScreen(cellX, cellY, out int screenX, out int screenY);
                        System.Diagnostics.Debug.WriteLine($"Screen X: {screenX}, Screen Y: {screenY}");

                        GGrid.SelectCell(x, y);
                        break;
                }
            }
        }

        private void Render()
        {
            Graphics.Clear();

            GGrid.Draw(Graphics);

            RenderText();

            Graphics.Present();
        }

        private void CreateText()
        {
            SDL.SDL_Color neonPink = new SDL.SDL_Color
            {
                r = 255,
                g = 16,
                b = 240,
                a = 255,
            };

            SDL.SDL_Color roseGold = new SDL.SDL_Color
            {
                r = 224,
                g = 191,
                b = 184,
                a = 255,
            };

            Font font = new Font("Assets/Fonts/Courier Prime.ttf", 48);

            GameText = new Text(font, "Hello World!", neonPink);
            GameText.CreateTexture(Graphics);
        }

        private void  RenderText()
        {
            GameText.Draw(Graphics);
        }
    }
}
