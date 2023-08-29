using SDL2;
using System.Diagnostics;

namespace WordAlchemy
{
    internal class App
    {
        public bool IsRunning { get; set; }

        private SDLGraphics Graphics { get; set; }

        private EventSystem Events { get; set; }

        private Map Map { get; set; }

        private Tools.FontViewer Viewer { get; set; }

        public App()
        {
            IsRunning = true;
            Graphics = SDLGraphics.Instance;
            
            if (!Graphics.Init())
            {
                IsRunning = false;
            }

            Events = EventSystem.Instance;
            WireEvents();

            Map = new Map(AppSettings.Instance.WindowWidth, AppSettings.Instance.WindowHeight, 240, 240);
            Map.GenerateMap();

            Viewer = new Tools.FontViewer(Terrain.Water);   
        }

        public void WireEvents()
        {
            Events.Listen(SDL.SDL_EventType.SDL_QUIT, (e) => this.IsRunning = false);
            Events.Listen(SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN, (e) => Debug.WriteLine($"Mouse Button: {e.button.button}"));
            Events.Listen(SDL.SDL_EventType.SDL_KEYDOWN, (e) => Debug.WriteLine($"Key Down: {e.key.keysym.sym}"));
            Events.Listen(SDL.SDL_EventType.SDL_KEYUP, (e) => Debug.WriteLine($"Key Up: {e.key.keysym.sym}"));
            Events.Listen(SDL.SDL_EventType.SDL_WINDOWEVENT, OnWindowResizedEvent);
        }

        public void Run()
        {
            while (IsRunning)
            {
                PollEvents();

                Map.Update();

                Render();
            }

            Graphics.CleanUp();
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

            Map.Draw();
            //Viewer.Draw(Graphics);
            //Graphics.Atlas?.Draw();

            Graphics.Present();
        }

        private void OnWindowResizedEvent(SDL.SDL_Event e)
        {  
            if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
            {
                AppSettings.Instance.WindowWidth = e.window.data1;
                AppSettings.Instance.WindowHeight = e.window.data2;
            }
        }
    }
}
