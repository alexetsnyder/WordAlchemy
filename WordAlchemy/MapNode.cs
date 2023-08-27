
namespace WordAlchemy
{
    internal class MapNode : Node
    {
        public TerrainInfo Info { get; set; }

        private SDLGraphics Graphics { get; set; }

        public MapNode(int id, int x, int y, TerrainInfo terrainInfo)
            : base(id, x, y)
        {
            Info = terrainInfo;

            Graphics = SDLGraphics.Instance;
        }

        public void DrawTo(IntPtr texture)
        {
            int x = X;
            int y = Y; 

            Graphics.DrawTextToTexture(texture, Info.Symbol, x, y, Info.Color, FontName.UNIFONT);
        }

        public void Draw()
        {
            int x = X;
            int y = Y;

            Graphics.DrawText(Info.Symbol, x, y, Info.Color, FontName.UNIFONT);
        }
    }
}
