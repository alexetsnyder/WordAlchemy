
namespace WordAlchemy.WorldGen
{
    public class MapNode : Node
    {
        public int X {  get; set; }
        public int Y { get; set; }

        public TerrainInfo Info { get; set; }

        public int? GroupID { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public MapNode(int id, int x, int y, TerrainInfo terrainInfo)
            : base(id)
        {
            Info = terrainInfo;
            
            X = x;
            Y = y;

            GroupID = null;

            GraphicSystem = GraphicSystem.Instance;
        }

        public List<MapNode> GetConnectedNodes()
        {
            List<MapNode> connectedNodeList = new List<MapNode>();

            foreach (Edge edge in EdgeList)
            {
                MapNode v1Node = (MapNode)edge.V1;
                MapNode v2Node = (MapNode)edge.V2;

                if (v1Node == this)
                {
                    connectedNodeList.Add(v2Node);
                }
                else
                {
                    connectedNodeList.Add(v1Node);
                }
            }

            return connectedNodeList;
        }

        public void DrawTo(IntPtr texture)
        {
            int x = X;
            int y = Y; 

            GraphicSystem.DrawTextToTexture(texture, Info.Symbol, x + Info.XMod, y + Info.YMod, Info.Color, AppSettings.Instance.MapFontName);
        }

        public void Draw()
        {
            int x = X;
            int y = Y;

            GraphicSystem.DrawText(Info.Symbol, x + Info.XMod, y + Info.YMod, Info.Color, AppSettings.Instance.MapFontName);
        }
    }
}
