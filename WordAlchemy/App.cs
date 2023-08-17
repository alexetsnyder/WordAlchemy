
using SDL2;

namespace WordAlchemy
{
    internal class App
    {
        public bool IsRunning { get; set; }

        private SDLGraphics Graphics { get; set; }

        private Grid GGrid { get; set; }

        public App()
        {
            IsRunning = true;
            Graphics = new SDLGraphics();
            
            if (!Graphics.Init())
            {
                IsRunning = false;
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

            Graphics.Present();
        }
    }
}
