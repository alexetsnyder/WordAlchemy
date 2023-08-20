
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

        public World(int windowWidth, int windowHeight, int rows, int cols)
        {
            WorldGrid = new Grid(windowWidth, windowHeight);
            Rows = rows;
            Cols = cols;

            Tiles = new Tile[Rows * Cols];

            MapGen = new WorldGen(Rows, Cols, 13);
            MapGen.GenerateHeightMap();
        }

        public void CreateTiles(SDLGraphics graphics)
        {
            Font font = new Font("Assets/Fonts/FreeMono.ttf", 24);
        
            for (int i = -Cols / 2; i < Cols / 2; i++)
            {
                for (int j = -Rows / 2; j < Rows / 2; j++)
                {
                    int x = i * WorldGrid.CellWidth;
                    int y = j * WorldGrid.CellWidth;

                    Text text = new Text(font, MapGen.GetTerrainType(i + (Cols / 2), j + (Rows / 2)), Colors.Black());
                    if (j > -5 && j < 5)
                    {
                        text = new Text(font, "~", Colors.Blue());
                    }

                    text.CreateTexture(graphics);
                    Tile tile = new Tile(text, x, y);

                    Tiles[(i + Cols / 2) * Rows + (j + Rows / 2)] = tile;
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
