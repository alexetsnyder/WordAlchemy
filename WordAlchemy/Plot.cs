
namespace WordAlchemy
{
    internal class Plot
    {
        public Node Node { get; set; }

        public int CenterX { get; set; }
        public int CenterY { get; set; }

        public TerrainInfo Info { get; set; }

        private SDLGraphics Graphics { get; set; }

        public Plot(Node node, int centerX, int centerY, TerrainInfo terrainInfo)
        {
            Node = node;
            CenterX = centerX;
            CenterY = centerY;
            Info = terrainInfo;

            Graphics = SDLGraphics.Instance;
        }

        public void Draw()
        {
            int x = CenterX;
            int y = CenterY;

            Graphics.DrawText(Info.Symbol, x, y, Info.Color, FontName.UNIFONT);
        }
    }
}
