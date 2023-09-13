
using SDL2;
using WordAlchemy.Grids;
using WordAlchemy.Settings;
using WordAlchemy.Systems;
using WordAlchemy.WorldGen;

namespace WordAlchemy.Viewers
{
    public class PlayerViewer : Viewer
    {
        public bool IsWorldGenerated {  get; private set; }

        public Player Player { get; set; }

        public Map Map { get; set; }

        public World? World { get; set; }

        private UI HUD { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public PlayerViewer(Map map, ViewWindow? srcViewWindow, ViewWindow? dstViewWindow)
            : base(srcViewWindow, dstViewWindow)
        {
            IsWorldGenerated = false;

            Map = map;
            World = null;
            Player = new Player();
            HUD = new UI();

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
                World = new World(Map, viewDistance: 2);

                Map.MapGen.GenerateWorld(World, cell, true);

                if (World.CenterChunk != null)
                {
                    if (SrcViewWindow != null)
                    {
                        SrcViewWindow.Size = World.CenterChunk.ChunkSize;
                    }

                    if (DstViewWindow != null)
                    {
                        DstViewWindow.Size = World.CenterChunk.ChunkSize;
                    }
                }

                IsWorldGenerated = true;
            }
            else
            {
                if (World != null && World.CenterChunk != null)
                {
                    Map.MapGen.GenerateWorld(World, cell, true); 
                }
            }

            if (World != null && World.CenterChunk != null)
            {
                int x = World.CenterChunk.ChunkPos.X + World.CenterChunk.ChunkSize.W / 2;
                int y = World.CenterChunk.ChunkPos.Y + World.CenterChunk.ChunkSize.H / 2;
                Player.WorldPos = new Point(x, y);
                
            }
        }

        public void Update()
        {
            HandleKeys();
            World?.CalculateChunksInView(Player.WorldPos);
            UpdateUI();
        }

        public void Draw()
        {
            if (SrcViewWindow != null && DstViewWindow != null && World != null)
            {
                SDL.SDL_Rect src = SrcViewWindow.GetViewRect();
                SDL.SDL_Rect dst = DstViewWindow.GetViewRect();

                World.Draw(ref src, ref dst);

                Point playerPos = Player.WorldPos - World.TopLeft;
                GraphicSystem.DrawText(Player.Symbol, playerPos.X, playerPos.Y, Colors.Red(), AppSettings.Instance.MapFontName);
            }

            HUD.Draw();
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
                    World.TopLeft = new Point(World.TopLeft.X, World.TopLeft.Y - speed);
                    Player.WorldPos = new Point(Player.WorldPos.X, Player.WorldPos.Y - speed);
                }
                if (key == InputSettings.Instance.PlayerDown)
                {
                    World.TopLeft = new Point(World.TopLeft.X, World.TopLeft.Y + speed);
                    Player.WorldPos = new Point(Player.WorldPos.X, Player.WorldPos.Y + speed);
                }
                if (key == InputSettings.Instance.PlayerLeft)
                {
                    World.TopLeft = new Point(World.TopLeft.X - speed, World.TopLeft.Y);
                    Player.WorldPos = new Point(Player.WorldPos.X - speed, Player.WorldPos.Y);
                }
                if (key == InputSettings.Instance.PlayerRight)
                {
                    World.TopLeft = new Point(World.TopLeft.X + speed, World.TopLeft.Y);
                    Player.WorldPos = new Point(Player.WorldPos.X + speed, Player.WorldPos.Y);
                }
            }
        }

        public void UpdateUI()
        {
            Cell? cell = Map.SelectedCell;
            if (cell.HasValue)
            {
                Group? group = Map.GetGroup(cell.Value);
                if (group != null)
                {
                    HUD.SetGroupTypeStr($"{group.Name} {group.Id}");
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
