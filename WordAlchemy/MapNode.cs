
namespace WordAlchemy
{
    public class MapNode : Node
    {
        public int X {  get; set; }
        public int Y { get; set; }

        public TerrainInfo Info { get; set; }

        public int? GroupID { get; set; }

        private SDLGraphics Graphics { get; set; }

        public MapNode(int id, int x, int y, TerrainInfo terrainInfo)
            : base(id)
        {
            Info = terrainInfo;
            
            X = x;
            Y = y;

            GroupID = null;

            Graphics = SDLGraphics.Instance;
        }

        public void DrawTo(IntPtr texture)
        {
            int x = X;
            int y = Y; 

            Graphics.DrawTextToTexture(texture, Info.Symbol, x + Info.XMod, y + Info.YMod, Info.Color, AppSettings.Instance.MapFontName);
        }

        public void Draw()
        {
            int x = X;
            int y = Y;

            Graphics.DrawText(Info.Symbol, x + Info.XMod, y + Info.YMod, Info.Color, AppSettings.Instance.MapFontName);
        }
    }
}
