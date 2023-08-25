
using SDL2;

namespace WordAlchemy
{
    internal class WorldMap
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int Rows { get; set; }
        public int Cols { get; set; }

        public Graph Graph { get; set; }

        public List<Plot> PlotList { get; set; }

        public MapGen MapGen { get; set; }

        public IntPtr MapTexture { get; set; }

        private static readonly int CharWidth = 9;
        private static readonly int CharHeight = 18;

        private SDLGraphics Graphics { get; set; }  

        public WorldMap(int width, int height, int rows, int cols)
        {
            Width = width;
            Height = height;

            Rows = rows; 
            Cols = cols; 

            Graph = new Graph();
            PlotList = new List<Plot>();

            Random random = new Random();
            MapGen = new MapGen(Rows, Cols, random.Next(0, 1000000));
            MapGen.GenerateMap();

            MapTexture = IntPtr.Zero;

            Graphics = SDLGraphics.Instance;
        }

        public void GenerateMap()
        {
            for (int i = 0; i < Rows;  i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    int x = j * CharWidth;
                    int y = i * CharHeight;

                    TerrainInfo terrain = MapGen.GetTerrain(i, j);

                    Node node = new Node(i * Cols + j, x, y);

                    Graph.NodeList.Add(node);
                    if (j != 0)
                    {
                        Edge newEdge = new Edge(PlotList[i * Cols + (j - 1)].Node, node);
                        Graph.AddEdge(newEdge);
                    }
                    if (i != 0)
                    {
                        Edge newEdge = new Edge(PlotList[(i - 1) * Cols + j].Node, node);
                        Graph.AddEdge(newEdge);
                    }                 

                    Plot plot = new Plot(node, terrain);
                    node.Reference = plot;

                    PlotList.Add(plot);
                }
            }

            GenerateMapTexture();
        }

        public void GenerateMapTexture()
        {
            MapTexture = Graphics.CreateTexture(Width, Height);

            foreach (Plot plot in PlotList)
            {
                plot.DrawTo(MapTexture);
            }
        }

        public void Draw()
        {
            SDL.SDL_Rect dest = new SDL.SDL_Rect
            {
                x = 0,
                y = 0,
                w = Width,
                h = Height,
            };

            Graphics.DrawTexture(MapTexture, ref dest);
        }
    }
}
