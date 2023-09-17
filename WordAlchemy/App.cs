using SDL2;
using System.Diagnostics;
using WordAlchemy.Settings;
using WordAlchemy.Systems;
using WordAlchemy.Viewers;
using WordAlchemy.WorldGen;
using WordAlchemy.Grids;
using WordAlchemy.GUI;

namespace WordAlchemy
{
    internal class App
    {
        public bool IsRunning { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        private EventSystem Events { get; set; }

        private MapViewer MapViewer { get; set; }

        private PlayerViewer PlayerViewer { get; set; }

        private UserInterface MapHUD { get; set; }

        private UserInterface PlayerHUD { get; set; }

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

            int chunkRows = 10;
            int chunkCols = 25;
            Point chunkSize = new Point(chunkCols * mapGen.CharSize.W, chunkRows * mapGen.CharSize.H);
            ChunkGen chunkGen = new ChunkGen(chunkRows, chunkCols, chunkSize);

            Map map = mapGen.GenerateMap();
            map.GenerateMapTexture();

            int viewDistance = 2;
            World world = new World(map, chunkGen, viewDistance);

            Point windowSize = new Point(AppSettings.Instance.WindowWidth, AppSettings.Instance.WindowHeight);

            ViewWindow DstViewWindow = new ViewWindow(new Point(0, 0), windowSize);
            ViewWindow SrcViewWindow = new ViewWindow(new Point(0, 0), windowSize);
            MapViewer = new MapViewer(world, SrcViewWindow, DstViewWindow);

            MapHUD = new UserInterface();
            TextElement groupType = new TextElement(MapViewer.GetGroupTypeStr);
            MapHUD.AddUIElement(groupType);

            DstViewWindow = new ViewWindow(new Point(0, 0), windowSize);
            SrcViewWindow = new ViewWindow(new Point(0, 0), windowSize);
            PlayerViewer = new PlayerViewer(world, SrcViewWindow, DstViewWindow);

            PlayerHUD = new UserInterface();
            groupType = new TextElement(PlayerViewer.GetGroupTypeStr);
            PlayerHUD.AddUIElement(groupType);

            GameSettings = GameSettings.Instance;
            GameSettings.State = GameState.MAP;
        }

        public void WireEvents()
        {
            Events.Listen(EventSystem.GLOBAL, SDL.SDL_EventType.SDL_QUIT, (e) => this.IsRunning = false);
            Events.Listen(EventSystem.GLOBAL, SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN, (e) => Debug.WriteLine($"Mouse Button: {e.button.button}"));
            Events.Listen(EventSystem.GLOBAL, SDL.SDL_EventType.SDL_WINDOWEVENT, OnWindowResizedEvent);
            Events.Listen(EventSystem.GLOBAL, SDL.SDL_EventType.SDL_KEYDOWN, OnKeyDown);
        }

        public void Run()
        {
            Time.Instance.Init();

            while (IsRunning)
            {
                Time.Instance.Update();

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
                MapHUD.Update();
            }
            else
            {
                PlayerViewer.Update();
                PlayerHUD.Update();
            }
        }

        private void Draw()
        {
            if (GameSettings.State == GameState.MAP)
            {
                MapViewer.Draw();
                MapHUD.Draw();
            }
            else
            {
                PlayerViewer.Draw();
                PlayerHUD.Draw();
            }
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

                if (GameSettings.State == GameState.PLAYER && MapViewer.World.Map.SelectedCell.HasValue)
                {
                    PlayerViewer.GenerateWorld();
                }
            }
        }
    }
}
