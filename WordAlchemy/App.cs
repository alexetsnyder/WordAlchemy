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

        private UI HUD { get; set; }

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

            Random random = new Random();
            MapGen mapGen = new MapGen(240, 240, random.Next(0, 1000000));

            Map = mapGen.GenerateMap();
            Map.DstViewWindow = new ViewWindow(0, 0, AppSettings.Instance.WindowWidth, AppSettings.Instance.WindowHeight);
            Map.SrcViewWindow = new ViewWindow(0, 0, AppSettings.Instance.WindowWidth, AppSettings.Instance.WindowHeight);
            Map.GenerateMapTexture();

            HUD = new UI(Map);

            Viewer = new Tools.FontViewer();   
        }

        public void WireEvents()
        {
            Events.Listen(SDL.SDL_EventType.SDL_QUIT, (e) => this.IsRunning = false);
            Events.Listen(SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN, (e) => Debug.WriteLine($"Mouse Button: {e.button.button}"));
            Events.Listen(SDL.SDL_EventType.SDL_WINDOWEVENT, OnWindowResizedEvent);
        }

        public void Run()
        {
            while (IsRunning)
            {
                PollEvents();

                Update();

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

            Draw();

            Graphics.Present();
        }

        private void Draw()
        {
            Map.Draw();
            HUD.Draw();
            //Viewer.Draw(Graphics);
            //Graphics.Atlas?.Draw();
        }

        private void Update()
        {
            Map.Update();
            HUD.Update();
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
