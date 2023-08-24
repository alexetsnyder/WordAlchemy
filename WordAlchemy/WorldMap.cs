
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

        public IntPtr MapTexture { get; set; }

        private SDLGraphics Graphics { get; set; }

        private static readonly int CharWidth = 9;
        private static readonly int CharHeight = 18;

        public WorldMap(int width, int height)
        {
            Width = width;
            Height = height;

            Rows = (Height - (Height % CharHeight)) / CharHeight;
            Cols = (Width - (Width % CharWidth)) / CharWidth;

            Graph = new Graph();
            PlotList = new List<Plot>();

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

                    TerrainInfo terrain = Terrain.Water;

                    Node node = new Node(i * Cols + j);

                    Graph.NodeList.Add(node);
                    if (j != 0)
                    {
                        Graph.EdgeList.Add(new Edge(PlotList[i * Cols + (j - 1)].Node, node));
                    }
                    if (i != 0)
                    {
                        Graph.EdgeList.Add(new Edge(PlotList[(i - 1) * Cols + j].Node, node));
                    }                 

                    Plot plot = new Plot(node, x, y, terrain);
                    PlotList.Add(plot);
                }
            }
        }

        public void Draw()
        {

        }
    }
}
