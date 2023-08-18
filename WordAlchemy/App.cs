using SDL2;

namespace WordAlchemy
{
    internal class App
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public bool IsRunning { get; set; }

        private SDLGraphics Graphics { get; set; }

        private World GameWorld { get; set; }

        public App(int windowWidth, int windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            IsRunning = true;
            Graphics = new SDLGraphics(windowWidth, windowHeight);
            
            if (!Graphics.Init())
            {
                IsRunning = false;
            }

            if (SDL_ttf.TTF_Init() < 0)
            {
                System.Diagnostics.Debug.WriteLine($"Couldn't initialize SDL TTF: {SDL.SDL_GetError()}");
                IsRunning = false;
            }

            GameWorld = new World(windowWidth, windowHeight, 100, 100);
            GameWorld.CreateTiles(Graphics);
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

                        GameWorld.WorldGrid.ScreenToCell(x, y, out int cellX, out int cellY);
                        System.Diagnostics.Debug.WriteLine($"Cell X: {cellX}, Cell Y: {cellY}");

                        GameWorld.WorldGrid.CellToScreen(cellX, cellY, out int screenX, out int screenY);
                        System.Diagnostics.Debug.WriteLine($"Screen X: {screenX}, Screen Y: {screenY}");

                        GameWorld.WorldGrid.SelectCell(x, y);
                        break;
                }
            }
        }

        private void Render()
        {
            Graphics.Clear();

            GameWorld.Draw(Graphics);

            Graphics.Present();
        }
    }
}
