
using SDL2;
using WordAlchemy.Helpers;
using WordAlchemy.WorldGen;

namespace WordAlchemy
{
    public class PlayerViewer : Viewer
    {
        public Player Player { get; set; }

        public Map Map { get; set; }

        public List<MapChunk> MapChunkList { get; private set; }

        public int ChunkRows { get; set; }
        public int ChunkCols { get; set; }

        private int TopLeftX { get; set; }
        private int TopLeftY { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public PlayerViewer(Map map, int chunkRows, int chunkCols, ViewWindow? srcViewWindow, ViewWindow? dstViewWindow)
            : base(srcViewWindow,  dstViewWindow)
        {
            Map = map;
            Player = new Player();

            MapChunkList = new List<MapChunk>();
            ChunkRows = chunkRows;
            ChunkCols = chunkCols;

            TopLeftX = 0;
            TopLeftY = 0;

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

        public void CreateMapChunkList()
        {
            if (Map.CurrentMapNode != null)
            {
                SetMapChunkList(Map.MapGen.GenerateMapChunks(Map.CurrentMapNode, 100, 100));
            }
        }

        public void SetMapChunkList(List<MapChunk> mapChunkList)
        {
            MapChunkList.Clear();

            MapChunkList.AddRange(mapChunkList);

            if (MapChunkList.Count > 0 )
            {
                int windowWidth = AppSettings.Instance.WindowWidth;
                int windowHeight = AppSettings.Instance.WindowHeight;

                MapChunk mapChunk = MapChunkList.First();
                TopLeftX = mapChunk.X - windowWidth / 2 + mapChunk.Width / 2;
                TopLeftY = mapChunk.Y - windowHeight / 2 + mapChunk.Height / 2;

                Player.X = mapChunk.X + mapChunk.Width / 2;
                Player.Y = mapChunk.Y + mapChunk.Height / 2;

                if (SrcViewWindow != null)
                {
                    SrcViewWindow.Width = mapChunk.Width;
                    SrcViewWindow.Height = mapChunk.Height;
                }

                if (DstViewWindow != null)
                {
                    DstViewWindow.Width = mapChunk.Width;
                    DstViewWindow.Height = mapChunk.Height;
                }
            }
        }

        public void Update()
        {
            HandleKeys();
            CheckChunks();
        }

        public void Draw()
        {
            if (SrcViewWindow != null && DstViewWindow != null)
            {
                SDL.SDL_Rect src = SrcViewWindow.GetViewRect();
                SDL.SDL_Rect dst = DstViewWindow.GetViewRect();

                foreach (MapChunk mapChunk in MapChunkList)
                {
                    dst.x = mapChunk.X - TopLeftX; 
                    dst.y = mapChunk.Y - TopLeftY;

                    GraphicSystem.SetDrawColor(Colors.Red());
                    GraphicSystem.DrawRect(ref dst);

                    mapChunk.Draw(ref src, ref dst);
                }

                GraphicSystem.DrawText(Player.Symbol, Player.X - TopLeftX, Player.Y - TopLeftY, Colors.Red(), AppSettings.Instance.MapFontName);
            }
        }

        private void HandleKeys()
        {
            if (SrcViewWindow == null)
            {
                return;
            }

            int speed = 5;

            foreach (var key in KeysPressedList)
            {
                if (key == InputSettings.Instance.MapUp)
                {
                    TopLeftY -= speed;
                    Player.Y -= speed;
                }
                if (key == InputSettings.Instance.MapDown)
                {
                    TopLeftY += speed;
                    Player.Y += speed;
                }
                if (key == InputSettings.Instance.MapLeft)
                {
                    TopLeftX -= speed;
                    Player.X -= speed;
                }
                if (key == InputSettings.Instance.MapRight)
                {
                    TopLeftX += speed;
                    Player.X += speed;
                }
            }
        }

        private void CheckChunks()
        {
            MapChunk? mapChunk = GetMapChunk(Player.X, Player.Y);
            if (mapChunk != null && mapChunk.MapNode != Map.CurrentMapNode)
            {
                Map.CurrentMapNode = mapChunk.MapNode;
                List<MapChunk> newChunkList = Map.MapGen.GenerateMapChunks(mapChunk.MapNode, ChunkRows, ChunkCols);

                MapChunkList.Clear();
                MapChunkList.AddRange(newChunkList);
            }
        }

        public MapChunk? GetMapChunk(int worldX, int worldY)
        {
            foreach (MapChunk chunk in MapChunkList)
            {
                int Ax = chunk.X; int Ay = chunk.Y;
                int Bx = chunk.X + chunk.Width; int By = chunk.Y;
                int Cx = chunk.X; int Cy = chunk.Y + chunk.Height;

                if (MathHelper.IsInRectangle(Ax, Ay, Bx, By, Cx, Cy, worldX, worldY))
                {
                    return chunk;
                }
            }

            return null;
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
