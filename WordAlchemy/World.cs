
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

            for (int i = -Rows / 2; i < Rows / 2; i++)
            {
                for (int j = -Cols / 2; j < Cols / 2; j++)
                {
                    int x = i * WorldGrid.CellWidth;
                    int y = j * WorldGrid.CellWidth;

                    Text text;
                    if (j > -5 && j < 5)
                    {
                        text = new Text(font, "~", Colors.Blue());
                    }
                    else
                    {
                        text = new Text(font, ".", Colors.Brown());
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
