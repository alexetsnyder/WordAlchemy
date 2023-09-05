
using SDL2;
using System.IO.Compression;
using WordAlchemy.WorldGen;

namespace WordAlchemy
{
    public class PlayerViewer : Viewer
    {
        public Player Player { get; set; }

        public Map Map { get; set; }

        public List<MapChunk> MapChunkList { get; private set; }

        public int ChunkWidth { get; set; }
        public int ChunkHeight { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public PlayerViewer(Map map, int chunkWidth, int chunkHeight, ViewWindow? srcViewWindow, ViewWindow? dstViewWindow)
            : base(srcViewWindow,  dstViewWindow)
        {
            Map = map;
            Player = new Player();

            MapChunkList = new List<MapChunk>();
            ChunkWidth = chunkWidth;
            ChunkHeight = chunkHeight;

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
                MapChunk mapChunk = MapChunkList.First();
                Player.X = mapChunk.X;
                Player.Y = mapChunk.Y;

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
        }

        public void Draw()
        {
            if (SrcViewWindow != null && DstViewWindow != null)
            {
                SDL.SDL_Rect src = SrcViewWindow.GetViewRect();
                SDL.SDL_Rect dst = DstViewWindow.GetViewRect();

                foreach (MapChunk mapChunk in MapChunkList)
                {
                    dst.x = mapChunk.X - Player.X; 
                    dst.y = mapChunk.Y - Player.Y;

                    GraphicSystem.SetDrawColor(Colors.Red());
                    GraphicSystem.DrawRect(ref dst);

                    mapChunk.Draw(ref src, ref dst);
                }
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
                    Player.Y -= speed;
                }
                if (key == InputSettings.Instance.MapDown)
                {
                    Player.Y += speed;
                }
                if (key == InputSettings.Instance.MapLeft)
                {
                    Player.X -= speed;
                }
                if (key == InputSettings.Instance.MapRight)
                {
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
