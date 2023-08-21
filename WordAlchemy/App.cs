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

        private EventSystem Events { get; set; }

        private World GameWorld { get; set; }

        private Tools.FontViewer Viewer { get; set; }

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

            Graphics.SetClearColor(Colors.Black());

            if (SDL_ttf.TTF_Init() < 0)
            {
                Debug.WriteLine($"Couldn't initialize SDL TTF: {SDL.SDL_GetError()}");
                IsRunning = false;
            }

            Events = EventSystem.Instance;
            WireEvents();

            GameWorld = new World(windowWidth, windowHeight, 50, 50);
            GameWorld.CreateTiles(Graphics);

            Viewer = new Tools.FontViewer(Terrain.SmallDoubleMountain, windowWidth, windowHeight);   
        }

        public void WireEvents()
        {
            Events.Listen(SDL.SDL_EventType.SDL_QUIT, (e) => this.IsRunning = false);
            Events.Listen(SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN, (e) => Debug.WriteLine($"Mouse Button: {e.button.button}"));
            Events.Listen(SDL.SDL_EventType.SDL_KEYDOWN, (e) => Debug.WriteLine($"Key Down: {e.key.keysym.sym}"));
            Events.Listen(SDL.SDL_EventType.SDL_KEYUP, (e) => Debug.WriteLine($"Key Up: {e.key.keysym.sym}"));
            Events.Listen(SDL.SDL_EventType.SDL_WINDOWEVENT, WindowResizedEvent);
        }

        public void Run()
        {
            while (IsRunning)
            {
                PollEvents();

                GameWorld.Update();

                Render();
            }

            Graphics.CleanUp();
            SDL_ttf.TTF_Quit();
        }

        private void PollEvents()
        {
            foreach (var e in Graphics.PollEvents())
            {
                Events.Invoke(e);
            }
        }

        private void Render()
        {
            Graphics.Clear();

            GameWorld.Draw(Graphics);
            //Viewer.Draw(Graphics);

            Graphics.Present();
        }

        private void WindowResizedEvent(SDL.SDL_Event e)
        {  
            if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
            {
                WindowWidth = e.window.data1;
                WindowHeight = e.window.data2;

                Graphics.WindowWidth = WindowWidth;
                Graphics.WindowHeight = WindowHeight;

                GameWorld.WorldGrid.SetWindowSize(WindowWidth, WindowHeight);

                Viewer.WindowWidth = WindowWidth;
                Viewer.WindowHeight = WindowHeight;
            }
        }
    }
}
