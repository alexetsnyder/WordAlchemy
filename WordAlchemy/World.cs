
using SDL2;

namespace WordAlchemy
{
    internal class World
    {
        public int Rows { get; set; }
        public int Cols { get; set; }

        public Tile[] Tiles { get; set; }

        public Grid WorldGrid { get; set; }

        public World(int windowWidth, int windowHeight, int rows, int cols)
        {
            WorldGrid = new Grid(windowWidth, windowHeight);
            Rows = rows;
            Cols = cols;

            Tiles = new Tile[Rows * Cols];
        }

        public void CreateTiles(SDLGraphics graphics)
        {
            Font font = new Font("Assets/Fonts/Courier Prime.ttf", 24);
            SDL.SDL_Color brown = new SDL.SDL_Color
            {
                r = 150,
                g =  75,
                b =   0,
                a = 255,
            };

            SDL.SDL_Color red = new SDL.SDL_Color
            {
                r = 250,
                g =   0,
                b =   0,
                a = 255,
            };

            SDL.SDL_Color blue = new SDL.SDL_Color
            {
                r =   0,
                g =   0,
                b = 255,
                a = 255,
            };

            for (int i = -Rows / 2; i < Rows / 2; i++)
            {
                for (int j = -Cols / 2; j < Cols / 2; j++)
                {
                    int x = i * WorldGrid.CellWidth;
                    int y = j * WorldGrid.CellWidth;

                    Text text;
                    if (x == -WorldGrid.WindowWidth / 2 && y == -WorldGrid.WindowHeight / 2)
                    {
                        text = new Text(font, ".", red);
                    }
                    else if (j > -2 && j < 2)
                    {
                        text = new Text(font, "~", blue);
                    }
                    else
                    {
                        text = new Text(font, ".", brown);
                    }

                    text.CreateTexture(graphics);
                    Tile tile = new Tile(text, x, y);

                    Tiles[(i + Rows / 2) * Cols + (j + Cols / 2)] = tile;
                }
            }
        }

        public void Draw(SDLGraphics graphics)
        {
            WorldGrid.Draw(graphics);
            foreach (Tile tile in Tiles)
            {
                if (WorldGrid.IsOnScreen(tile.GridX, tile.GridY))
                {
                    tile.Draw(graphics, WorldGrid);
                }  
            }
        }
    }
}
