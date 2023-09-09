
using SDL2;
using WordAlchemy.WorldGen;

namespace WordAlchemy.Viewers
{
    public class PlayerViewer : Viewer
    {
        public bool IsWorldGenerated {  get; private set; }

        public Player Player { get; set; }

        public Map Map { get; set; }

        public World? World { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public PlayerViewer(Map map, ViewWindow? srcViewWindow, ViewWindow? dstViewWindow)
            : base(srcViewWindow, dstViewWindow)
        {
            IsWorldGenerated = false;

            Map = map;
            World = null;
            Player = new Player();

            KeysPressedList = new List<SDL.SDL_Keycode>();

            GraphicSystem = GraphicSystem.Instance;

            WireEvents();
        }

        private void WireEvents()
        {
            EventSystem eventSystem = EventSystem.Instance;
            eventSystem.Listen((int)GameState.PLAYER, SDL.SDL_EventType.SDL_KEYDOWN, OnKeyDown);
            eventSystem.Listen((int)GameState.PLAYER, SDL.SDL_EventType.SDL_KEYUP, OnKeyUp);
        }

        public void GenerateWorld(Cell cell)
        {
            if (!IsWorldGenerated)
            {
                World = Map.MapGen.GenerateWorld(Map, cell);

                if (World.CenterChunk != null)
                {
                    Player.X = World.CenterChunk.X + World.CenterChunk.Width / 2;
                    Player.Y = World.CenterChunk.Y + World.CenterChunk.Height / 2;

                    if (SrcViewWindow != null)
                    {
                        SrcViewWindow.Width = World.CenterChunk.Width;
                        SrcViewWindow.Height = World.CenterChunk.Height;
                    }

                    if (DstViewWindow != null)
                    {
                        DstViewWindow.Width = World.CenterChunk.Width;
                        DstViewWindow.Height = World.CenterChunk.Height;
                    }
                }

                IsWorldGenerated = true;
            }
            else
            {
                if (World != null && World.CenterChunk != null)
                {
                    Map.MapGen.RegenerateWorld(World, Map, cell, true);

                    Player.X = World.CenterChunk.X + World.CenterChunk.Width / 2;
                    Player.Y = World.CenterChunk.Y + World.CenterChunk.Height / 2;
                }
            }
            
        }

        public void Update()
        {
            HandleKeys();
            World?.CalculateChunksInView(Player.X, Player.Y);
        }

        public void Draw()
        {
            if (SrcViewWindow != null && DstViewWindow != null && World != null)
            {
                SDL.SDL_Rect src = SrcViewWindow.GetViewRect();
                SDL.SDL_Rect dst = DstViewWindow.GetViewRect();

                World.Draw(ref src, ref dst);

                GraphicSystem.DrawText(Player.Symbol, Player.X - World.TopLeftX, Player.Y - World.TopLeftY, Colors.Red(), AppSettings.Instance.MapFontName);
            }
        }

        private void HandleKeys()
        {
            if (SrcViewWindow == null || World == null)
            {
                return;
            }

            int speed = 5;

            foreach (var key in KeysPressedList)
            {
                if (key == InputSettings.Instance.PlayerUp)
                {
                    World.TopLeftY -= speed;
                    Player.Y -= speed;
                }
                if (key == InputSettings.Instance.PlayerDown)
                {
                    World.TopLeftY += speed;
                    Player.Y += speed;
                }
                if (key == InputSettings.Instance.PlayerLeft)
                {
                    World.TopLeftX -= speed;
                    Player.X -= speed;
                }
                if (key == InputSettings.Instance.PlayerRight)
                {
                    World.TopLeftX += speed;
                    Player.X += speed;
                }
            }
        }

        public void OnKeyDown(SDL.SDL_Event e)
        {
            if (!KeysPressedList.Contains(e.key.keysym.sym))
            {
                KeysPressedList.Add(e.key.keysym.sym);
            }
        }

        public void OnKeyUp(SDL.SDL_Event e)
        {
            KeysPressedList.Remove(e.key.keysym.sym);
        }
    }
}
