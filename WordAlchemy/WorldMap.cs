
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

        private static readonly int CharWidth = 9;
        private static readonly int CharHeight = 18;

        private SDLGraphics Graphics { get; set; }  

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
        }

        public void Draw()
        {
            foreach (Edge edge in Graph.EdgeList)
            {
                Graphics.SetDrawColor(Colors.Green());
                Graphics.DrawLine(edge.V1.X, edge.V1.Y, edge.V2.X, edge.V2.Y);
            }

            foreach (Node node in Graph.NodeList)
            {
                Graphics.SetDrawColor(Colors.Red());
                Graphics.DrawPoint(node.X, node.Y);
            }

            foreach (Plot plot in PlotList)
            {
                plot.Draw();
            }
        }
    }
}
