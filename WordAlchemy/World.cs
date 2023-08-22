
using SDL2;

namespace WordAlchemy
{
    internal class World
    {
        public int Rows { get; set; }
        public int Cols { get; set; }

        public Tile[] Tiles { get; set; }

        public Grid WorldGrid { get; set; }
        
        public WorldGen MapGen { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        public World(int windowWidth, int windowHeight, int rows, int cols)
        {
            WorldGrid = new Grid(windowWidth, windowHeight);
            WorldGrid.CellWidth = 10;
            Rows = rows;
            Cols = cols;

            Tiles = new Tile[Rows * Cols];

            Random random = new Random();
            MapGen = new WorldGen(Rows, Cols, random.Next(0, 1000000));
            MapGen.GenerateHeightMap();

            KeysPressedList = new List<SDL.SDL_Keycode>();

            WireEvents();
        }

        private void WireEvents()
        {
            EventSystem eventSystem = EventSystem.Instance;
            eventSystem.Listen(SDL.SDL_EventType.SDL_KEYDOWN, KeyDownEvent);
            eventSystem.Listen(SDL.SDL_EventType.SDL_KEYUP, KeyUpEvent);
        }

        public void CreateTiles(SDLGraphics graphics)
        {
            Font font = new Font("unifont", "Assets/Fonts/unifont.ttf", 18);
        
            for (int i = -Cols / 2; i < Cols / 2; i++)
            {
                for (int j = -Rows / 2; j < Rows / 2; j++)
                {
                    int x = i * WorldGrid.CellWidth;
                    int y = j * WorldGrid.CellWidth;

                    TerrainInfo terrain = MapGen.GetTerrain(i + (Cols / 2), j + (Rows / 2));

                    Text text = new Text(font, terrain.Symbol, terrain.Color);

                    Tile tile = new Tile(text, terrain, x, y);

                    Tiles[(i + Cols / 2) * Rows + (j + Rows / 2)] = tile;
                }
            }
        }

        public void Update()
        {
            HandelKeys();
        }

        public void Draw()
        {
            WorldGrid.Draw();
            foreach (Tile tile in Tiles)
            {
                if (WorldGrid.IsOnScreen(tile.GridX, tile.GridY))
                {
                    tile.Draw(WorldGrid);
                }  
            }
        }

        private void HandelKeys()
        {
            int speed = 5;

            foreach (var key in KeysPressedList)
            {
                if (key == SDL.SDL_Keycode.SDLK_w)
                {
                    WorldGrid.OriginOffsetY += speed;
                }
                if (key == SDL.SDL_Keycode.SDLK_s)
                {
                    WorldGrid.OriginOffsetY -= speed;
                }
                if (key == SDL.SDL_Keycode.SDLK_a)
                {
                    WorldGrid.OriginOffsetX += speed;
                }
                if (key == SDL.SDL_Keycode.SDLK_d)
                {
                    WorldGrid.OriginOffsetX -= speed;
                }
            }
        }

        public void KeyDownEvent(SDL.SDL_Event e)
        {
            if (!KeysPressedList.Contains(e.key.keysym.sym))
            {
                KeysPressedList.Add(e.key.keysym.sym);
            }
        }

        public void KeyUpEvent(SDL.SDL_Event e)
        {
            KeysPressedList.Remove(e.key.keysym.sym);
        }
    }
}
