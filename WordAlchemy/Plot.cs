
namespace WordAlchemy
{
    internal class Plot
    {
        public Node Node { get; set; }

        public TerrainInfo Info { get; set; }

        private SDLGraphics Graphics { get; set; }

        public Plot(Node node, TerrainInfo terrainInfo)
        {
            Node = node;
            Info = terrainInfo;

            Graphics = SDLGraphics.Instance;
        }

        public void Draw()
        {
            int x = Node.X;
            int y = Node.Y;

            Graphics.DrawText(Info.Symbol, x, y, Info.Color, FontName.UNIFONT);
        }
    }
}
