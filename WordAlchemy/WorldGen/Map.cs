
using SDL2;
using WordAlchemy.Helpers;

namespace WordAlchemy.WorldGen
{
    public class Map
    {
        public Graph? Graph { get; set; }

        public List<Group> GroupList { get; set; }

        public MapGen MapGen { get; set; }

        public MapNode? CurrentMapNode { get; set; }

        private IntPtr MapTexture { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public Map(MapGen mapGen)
        {
            MapGen = mapGen;

            Graph = null;
            GroupList = new List<Group>();

            CurrentMapNode = null;

            MapTexture = IntPtr.Zero;

            GraphicSystem = GraphicSystem.Instance;
        }

        public void GenerateMapTexture()
        {
            MapTexture = GraphicSystem.CreateTexture(MapGen.Width, MapGen.Height);

            if (Graph != null)
            {
                foreach (MapNode mapNode in Graph.NodeList)
                {
                    mapNode.DrawTo(MapTexture);
                }
            } 
        }

        public void Draw(ref SDL.SDL_Rect src,  ref SDL.SDL_Rect dest)
        {
            GraphicSystem.DrawTexture(MapTexture, ref src, ref dest);
        }

        public Group? GetGroup(int groupId)
        {
            foreach (Group group in GroupList)
            {
                if (group.Id == groupId)
                {
                    return group;
                }
            }

            return null;
        }

        public MapNode? GetMapNode(int worldX, int worldY)
        {
            if (Graph != null)
            {
                foreach (MapNode mapNode in Graph.NodeList)
                {
                    int Ax = mapNode.X, Ay = mapNode.Y;
                    int Bx = Ax + MapGen.CharWidth, By = Ay;
                    int Cx = Ax + MapGen.CharWidth, Cy = Ay + MapGen.CharHeight;

                    if (MathHelper.IsInRectangle(Ax, Ay, Bx, By, Cx, Cy, worldX, worldY))
                    {
                        return mapNode;
                    }
                }
            }

            return null;
        }
    }
}
