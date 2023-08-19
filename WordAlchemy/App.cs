using SDL2;
using System.Diagnostics;

namespace WordAlchemy
{
    internal class App
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public bool IsRunning { get; set; }

        private SDLGraphics Graphics { get; set; }

        private World GameWorld { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

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
                Debug.WriteLine($"Couldn't initialize SDL TTF: {SDL.SDL_GetError()}");
                IsRunning = false;
            }

            GameWorld = new World(windowWidth, windowHeight, 100, 100);
            GameWorld.CreateTiles(Graphics);

            KeysPressedList = new List<SDL.SDL_Keycode>();
        }

        public void Run()
        {
            while (IsRunning)
            {
                PollEvents();
                HandleKeys();

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
                        Debug.WriteLine($"{e.button.button}");
                     
                        if (e.button.button == SDL.SDL_BUTTON_LEFT)
                        {
                            SDL.SDL_GetMouseState(out int x, out int y);
                            Debug.WriteLine($"Mouse X: {x}, Mouse Y: {y}");

                            GameWorld.WorldGrid.ScreenToCell(x, y, out int cellX, out int cellY);
                            Debug.WriteLine($"Cell X: {cellX}, Cell Y: {cellY}");

                            GameWorld.WorldGrid.CellToScreen(cellX, cellY, out int screenX, out int screenY);
                            Debug.WriteLine($"Screen X: {screenX}, Screen Y: {screenY}");

                            GameWorld.WorldGrid.SelectCell(x, y);
                        }
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        Debug.WriteLine(e.key.keysym.sym);
                        if (!KeysPressedList.Contains(e.key.keysym.sym))
                        {
                            KeysPressedList.Add(e.key.keysym.sym);
                        }
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        Debug.WriteLine(e.key.keysym.sym);
                        KeysPressedList.Remove(e.key.keysym.sym);
                        break;
                }
            }
        }

        private void HandleKeys()
        {
            foreach (var key in KeysPressedList)
            {
                if (key == SDL.SDL_Keycode.SDLK_w)
                {
                    GameWorld.WorldGrid.OriginOffsetY += 1;
                }
                if (key == SDL.SDL_Keycode.SDLK_s)
                {
                    GameWorld.WorldGrid.OriginOffsetY -= 1;
                }
                if (key == SDL.SDL_Keycode.SDLK_a)
                {
                    GameWorld.WorldGrid.OriginOffsetX += 1;
                }
                if (key == SDL.SDL_Keycode.SDLK_d)
                {
                    GameWorld.WorldGrid.OriginOffsetX -= 1;
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
