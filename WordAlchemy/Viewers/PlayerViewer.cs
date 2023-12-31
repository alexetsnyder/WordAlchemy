﻿using SDL2;
using WordAlchemy.Grids;
using WordAlchemy.Settings;
using WordAlchemy.Systems;
using WordAlchemy.WorldGen;

namespace WordAlchemy.Viewers
{
    public class PlayerViewer : Viewer
    {
        public Player Player { get; set; }

        public World World { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public PlayerViewer(World world, ViewWindow? srcViewWindow, ViewWindow? dstViewWindow)
            : base(srcViewWindow, dstViewWindow)
        {
            if (SrcViewWindow != null)
            {
                SrcViewWindow.Size = world.ChunkGen.ChunkSize;
            }
            if (DstViewWindow != null)
            {
                DstViewWindow.Size = world.ChunkGen.ChunkSize;
            }

            World = world;
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

        public void GenerateWorld()
        {
            World.GenerateWorld(true);

            if (World.CenterChunk != null)
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

        public string GetGroupTypeStr()
        {
            string groupTypeStr = "";

            Cell? cell = World?.CenterChunk?.MapCell;
            if (cell.HasValue)
            {
                Group? group = World?.Map.GetGroup(cell.Value);
                if (group != null)
                {
                    groupTypeStr = $"{group.Name} {group.Id}";
                }
            }

            return groupTypeStr;
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
