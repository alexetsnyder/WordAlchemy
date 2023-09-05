using SDL2;
using System.Diagnostics;
using WordAlchemy.WorldGen;

namespace WordAlchemy
{
    internal class App
    {
        public bool IsRunning { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        private EventSystem Events { get; set; }

        private MapViewer MapViewer { get; set; }

        private PlayerViewer PlayerViewer { get; set; }

        private UI HUD { get; set; }

        private Tools.FontViewer Viewer { get; set; }

        private GameSettings GameSettings { get; set; }

        public App()
        {
            IsRunning = true;
            GraphicSystem = GraphicSystem.Instance;

            if (!GraphicSystem.Init())
            {
                IsRunning = false;
            }

            Events = EventSystem.Instance;
            WireEvents();

            Random random = new Random();
            MapGen mapGen = new MapGen(240, 240, random.Next(0, 1000000));

            Map map = mapGen.GenerateMap();
            map.GenerateMapTexture();

            ViewWindow DstViewWindow = new ViewWindow(0, 0, AppSettings.Instance.WindowWidth, AppSettings.Instance.WindowHeight);
            ViewWindow SrcViewWindow = new ViewWindow(0, 0, AppSettings.Instance.WindowWidth, AppSettings.Instance.WindowHeight);
            MapViewer = new MapViewer(map, SrcViewWindow, DstViewWindow);

            DstViewWindow = new ViewWindow(0, 0, AppSettings.Instance.WindowWidth, AppSettings.Instance.WindowHeight);
            SrcViewWindow = new ViewWindow(0, 0, AppSettings.Instance.WindowWidth, AppSettings.Instance.WindowHeight);
            PlayerViewer = new PlayerViewer(map, 100, 100, SrcViewWindow, DstViewWindow);

            HUD = new UI(MapViewer);

            Viewer = new Tools.FontViewer();

            GameSettings = GameSettings.Instance;
            GameSettings.State = GameState.MAP;
        }

        public void WireEvents()
        {
            Events.Listen(-1, SDL.SDL_EventType.SDL_QUIT, (e) => this.IsRunning = false);
            Events.Listen(-1, SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN, (e) => Debug.WriteLine($"Mouse Button: {e.button.button}"));
            Events.Listen(-1, SDL.SDL_EventType.SDL_WINDOWEVENT, OnWindowResizedEvent);
            Events.Listen(-1, SDL.SDL_EventType.SDL_KEYDOWN, OnKeyDown);
        }

        public void Run()
        {
            while (IsRunning)
            {
                PollEvents();

                Update();

                Render();
            }

            GraphicSystem.CleanUp();
        }

        private void PollEvents()
        {
            foreach (var e in GraphicSystem.PollEvents())
            {
                Events.Invoke((int)GameSettings.State, e);
            }
        }

        private void Render()
        {
            GraphicSystem.Clear();

            Draw();

            GraphicSystem.Present();
        }

        private void Update()
        {
            if (GameSettings.State == GameState.MAP)
            {
                MapViewer.Update();
            }
            else
            {
                PlayerViewer.Update();
            }

            HUD.Update();
        }

        private void Draw()
        {
            if (GameSettings.State == GameState.MAP)
            {
                MapViewer.Draw();
            }
            else
            {
                PlayerViewer.Draw();
            }

            HUD.Draw();
            //Viewer.Draw(Graphics);
            //Graphics.Atlas?.Draw();
        }

        private void OnWindowResizedEvent(SDL.SDL_Event e)
        {
            if (e.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
            {
                AppSettings.Instance.WindowWidth = e.window.data1;
                AppSettings.Instance.WindowHeight = e.window.data2;
            }
        }

        private void OnKeyDown(SDL.SDL_Event e)
        {
            if (e.key.keysym.sym == InputSettings.Instance.MapButton)
            {
                GameSettings.State = (GameSettings.State == GameState.MAP) ? GameState.PLAYER : GameState.MAP;

                if (GameSettings.State == GameState.PLAYER)
                {
                    PlayerViewer.CreateMapChunkList();
                }
            }
        }
    }
}
