
using SDL2;
using System.Diagnostics;
using WordAlchemy.Helpers;

namespace WordAlchemy
{
    public class Map
    {
        public Graph? Graph { get; set; }

        public Graph? WorldGraph { get; set; }

        public List<Group> GroupList { get; set; }

        public MapGen MapGen { get; set; }

        private MapState MapState { get; set; }

        private IntPtr MapTexture { get; set; }

        private IntPtr WorldTexture { get; set; }

        private SDLGraphics Graphics { get; set; }

        public Map(MapGen mapGen)
        {
            MapState = MapState.MAP;
            MapGen = mapGen;

            Graph = null;
            WorldGraph = null;
            GroupList = new List<Group>();

            MapTexture = IntPtr.Zero;

            Graphics = SDLGraphics.Instance;
        }

        public void ToggleMapState()
        {
            MapState = (MapState == MapState.MAP) ? MapState.WORLD : MapState.MAP;
        }

        public void GenerateMapTexture()
        {
            MapTexture = Graphics.CreateTexture(MapGen.Width, MapGen.Height);

            if (Graph != null)
            {
                foreach (MapNode mapNode in Graph.NodeList)
                {
                    mapNode.DrawTo(MapTexture);
                }
            } 
        }

        public void CreateWorld(TerrainInfo terrain)
        {
            MapState = MapState.WORLD;
            WorldGraph = MapGen.GenerateWorldGraph(terrain);
            GenerateWorldTexture();
        }

        public void GenerateWorldTexture()
        {
            WorldTexture = Graphics.CreateTexture(MapGen.Width, MapGen.Height);

            if (WorldGraph != null)
            {
                foreach (MapNode mapNode in WorldGraph.NodeList)
                {
                    mapNode.DrawTo(WorldTexture);
                }
            }
        }

        public void Draw(SDL.SDL_Rect src,  SDL.SDL_Rect dest)
        {
            if (MapState == MapState.MAP)
            {
                Graphics.DrawTexture(MapTexture, ref src, ref dest);
            }
            else
            {
                Graphics.DrawTexture(WorldTexture, ref src, ref dest);
            }      
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
                    int Cx = Ax, Cy = Ay + MapGen.CharHeight;

                    if (MathHelper.IsInRectangle(Ax, Ay, Bx, By, Cx, Cy, worldX, worldY))
                    {
                        return mapNode;
                    }
                }
            }

            return null;
        }
    }

    public enum MapState
    {
        MAP,
        WORLD,
    }
}
