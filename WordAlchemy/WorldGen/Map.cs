
using SDL2;
using WordAlchemy.Helpers;

namespace WordAlchemy.WorldGen
{
    public class Map
    {
        public Grid Grid { get; set; }

        public byte[] GridCells { get; set; } 

        public Graph? Graph { get; set; }

        public List<Group> GroupList { get; set; }

        public MapGen MapGen { get; set; }

        public MapNode? CurrentMapNode { get; set; }

        private IntPtr MapTexture { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public Map(MapGen mapGen)
        {
            MapGen = mapGen;

            Grid = new Grid(MapGen.CharWidth, MapGen.CharHeight);
            GridCells = new byte[MapGen.Rows * MapGen.Cols];

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
                //foreach (MapNode mapNode in Graph.NodeList)
                //{
                //    mapNode.DrawTo(MapTexture);
                //}

                foreach (var tuple in GetCells())
                {
                    DrawTerrain(tuple.Item1, tuple.Item2);
                }
            } 
        }

        public IEnumerable<Tuple<byte, Cell>> GetCells()
        {
            for (int i = 0; i < MapGen.Rows; i++)
            {
                for (int j = 0; j < MapGen.Cols; j++)
                {
                    yield return new Tuple<byte, Cell>(GridCells[i * MapGen.Cols + j],  Grid.GetCell(i, j));
                }
            }
        }

        public bool IsCellGrouped(int i, int j)
        {
            foreach (Group group in GroupList)
            {
                if (group.IsCellInGroup(i, j))
                {
                    return true;
                }
            }
            return false;
        }

        private void DrawTerrain(byte terrain, Cell cell)
        {
            TerrainInfo terrainInfo = Terrain.TerrainArray[terrain];

            int x = cell.X + terrainInfo.XMod;
            int y = cell.Y + terrainInfo.YMod;

            GraphicSystem.DrawTextToTexture(MapTexture, terrainInfo.Symbol, x, y, terrainInfo.Color, AppSettings.Instance.MapFontName);
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

        public Group? GetGroup(int i, int j)
        {
            foreach (Group group in GroupList)
            {
                if (group.IsCellInGroup(i, j))
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
