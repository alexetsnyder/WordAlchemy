
using SDL2;

namespace WordAlchemy
{
    internal class App
    {
        public bool IsRunning { get; set; }

        private SDLGraphics Graphics { get; set; }

        private Grid GGrid { get; set; }

        private IntPtr TextTexture { get; set; }

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
            SDL.SDL_Color color = new SDL.SDL_Color
            {
                r =   0,
                g =   0,
                b = 255,
                a = 255,
            };
            IntPtr font = SDL_ttf.TTF_OpenFont("Assets/Fonts/Courier Prime.ttf", 48);

            IntPtr surface = SDL_ttf.TTF_RenderUTF8_Blended(font, "Hello World!", color);

            TextTexture = Graphics.CreateTextureFromSurface(surface);

            SDL.SDL_FreeSurface(surface);
        }

        private void  RenderText()
        {
            SDL.SDL_QueryTexture(TextTexture, out _, out _, out int w, out int h);
            Graphics.DrawTexture(TextTexture, 520 - w / 2, 440 - h / 2);
        }
    }
}
