
using SDL2;
using WordAlchemy.Helpers;

namespace WordAlchemy
{
    internal class MapGen
    {
        public int Width
        {
            get
            {
                return Cols * CharWidth;
            }
        }

        public int Height
        {
            get
            {
                return Rows * CharHeight;
            }
        }

        public int Rows { get; set; }
        public int Cols { get; set; }

        public int Seed { get; set; }

        public int CharWidth { get; private set; }
        public int CharHeight { get; private set; }

        private FastNoiseLite Noise { get; set; }
        private float[] HeightMap { get; set; }

        public MapGen(int rows, int cols, int seed)
        {
            Rows = rows;
            Cols = cols;

            Seed = seed;
            Noise = new FastNoiseLite(seed);
            Noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            Noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            SDLGraphics.Instance.SizeText(Terrain.Water.Symbol, FontName.IBM_VGA_8X14, out int width, out int height);
            CharWidth = width;
            CharHeight = height;

            HeightMap = new float[Cols * Rows];
        }

        public Map GenerateMap()
        {
            GenerateHeightMap();

            Map map = new Map(this, Rows, Cols);
            map.Graph = GenerateGraph();
            map.GroupList = GroupTerrain(map);

            GenerateRivers(map);

            return map;
        }

        private void GenerateHeightMap()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    float noise = Noise.GetNoise(i, j);
                    float remapNoise = MathHelper.Remap(noise, -1.0f, 1.0f, 0.0f, 100.0f);
                    float fallOffValue = MathHelper.SigmoidFallOffMapCircular(j, i, Cols, Rows);

                    HeightMap[i * Cols + j] = remapNoise * fallOffValue;
                }
            }
        }

        private Graph GenerateGraph()
        {
            Graph graph = new Graph();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    int x = j * CharWidth;
                    int y = i * CharHeight;

                    TerrainInfo terrain = GetTerrain(i, j);

                    MapNode mapNode = new MapNode(i * Cols + j, x, y, terrain);

                    graph.AddNode(mapNode);
                    if (j != 0)
                    {
                        Edge newEdge = new Edge(graph.NodeList[i * Cols + (j - 1)], mapNode);
                        graph.AddEdge(newEdge);
                    }
                    if (i != 0)
                    {
                        Edge newEdge = new Edge(graph.NodeList[(i - 1) * Cols + j], mapNode);
                        graph.AddEdge(newEdge);
                    }
                }
            }
            return graph;
        }

        private List<Group> GroupTerrain(Map map)
        {
            List<Group> groupList = new List<Group>();

            if (map.Graph != null)
            {
                int groupIndex = 0;
                foreach (MapNode mapNode in map.Graph.NodeList)
                {
                    if (!mapNode.GroupID.HasValue)
                    {
                        TerrainType type = mapNode.Info.Type;
                        Group group = new Group(groupIndex, type, type.ToString());

                        FillGroup(mapNode, group);

                        groupList.Add(group);
                        groupIndex++;
                    }
                }
            }

            return groupList;
        }

        private void FillGroup(MapNode mapNode, Group group)
        {
            Stack<MapNode> stack = new Stack<MapNode>(new MapNode[] { mapNode }); 
            
            while (stack.Count > 0)
            {
                MapNode currentNode = stack.Pop();
                if (!currentNode.GroupID.HasValue)
                {
                    group.MapNodeList.Add(currentNode);
                    currentNode.GroupID = group.Id;

                    foreach (Edge edge in currentNode.EdgeList)
                    {
                        if (edge.V1 == currentNode)
                        {
                            if (edge.V2 is MapNode otherNode && otherNode.Info.Equals(currentNode.Info))
                            {
                                stack.Push(otherNode);
                            }
                        }
                        else
                        {
                            if (edge.V1 is MapNode otherNode && otherNode.Info.Equals(currentNode.Info))
                            {
                                stack.Push(otherNode);
                            }
                        }
                    }
                }
            } 
        }

        private void GenerateRivers(Map map)
        {
            foreach (Group group in map.GroupList)
            {
                if (group.Type == TerrainType.MOUNTAIN)
                {
                    MapNode mapNode = GetMaxHeight(group.MapNodeList);
                    Group riverGroup = new Group(map.GroupList.Last().Id + 1, TerrainType.WATER, TerrainType.WATER.ToString());

                    if (mapNode.X <= Width / 2)
                    {
                        GenerateRiverRecursive(riverGroup, mapNode, (x, y) => x <= y);
                    }
                    else
                    {
                        GenerateRiverRecursive(riverGroup, mapNode, (x, y) => x >= y);
                    }     
                }
            }
        }

        private void GenerateRiverRecursive(Group group, MapNode mapNode, Func<int, int, bool> compare)
        {
            TerrainType type = mapNode.Info.Type;
            if (type != TerrainType.WATER)
            {
                if (type != TerrainType.MOUNTAIN)
                {
                    mapNode.Info = Terrain.Water;
                }

                mapNode.GroupID = group.Id;
                group.MapNodeList.Add(mapNode);

                List<MapNode> possibleNodes = new List<MapNode>();
                foreach (Edge edge in mapNode.EdgeList)
                {
                    if (edge.V1 == mapNode)
                    {
                        if (edge.V2 is MapNode newNode && compare(newNode.X, mapNode.X))
                        {
                            if (newNode.GroupID != group.Id)
                            {
                                possibleNodes.Add(newNode);
                            }  
                        }
                    }
                    else
                    {
                        if (edge.V1 is MapNode newNode && compare(newNode.X, mapNode.X))
                        {
                            if (newNode.GroupID != group.Id)
                            {
                                possibleNodes.Add(newNode);
                            }
                        }
                    }
                }

                MapNode? minMapNode = GetMinHeight(possibleNodes);
                if (minMapNode != null)
                {
                    GenerateRiverRecursive(group, minMapNode, compare);
                }
            }
        }

        private MapNode? GetMinHeight(List<MapNode> mapNodeList)
        {
            float minHeight = float.MaxValue;
            MapNode? minMapNode = null;

            foreach (MapNode mapNode in mapNodeList)
            {
                float height = HeightMap[mapNode.Id];
                if (height < minHeight)
                {
                    minHeight = height;
                    minMapNode = mapNode;
                }
            }

            return minMapNode;
        }

        private MapNode GetMaxHeight(List<MapNode> mapNodeList)
        {
            return GetMaxHeights(mapNodeList, 1).First();
        }

        private MapNode[] GetMaxHeights(List<MapNode> mapNodeList, int n = 1)
        {
            float[] maxValues = new float[n];
            MapNode[] mapNodeArray = new MapNode[n];

            foreach (MapNode mapNode in mapNodeList)
            {
                float heightValue = HeightMap[mapNode.Id];

                for (int k = 0; k < n; k++)
                {
                    if (heightValue > maxValues[k])
                    {
                        maxValues[k] = heightValue;
                        mapNodeArray[k] = mapNode;
                        break;
                    }
                }
            }

            return mapNodeArray;
        }

        public TerrainInfo GetTerrain(int i, int j)
        {
            float heightValue = HeightMap[i * Cols + j];

            TerrainInfo terrain;
            if (heightValue < 20.0f)
            {
                terrain = Terrain.Water;
            }
            else if (heightValue < 25.0f)
            {
                terrain = Terrain.Sand;
            }
            else if (heightValue < 40.0f)
            {
                terrain = Terrain.Grass;
            }
            else if (heightValue < 60.0f)
            {
                terrain = Terrain.Hill;
            }
            else //(heightValue < 100.0f)
            {
                terrain = Terrain.Mountain;
            }

            return terrain;
        }
    }

    public static class Terrain
    {
        public static TerrainInfo Water = new TerrainInfo(TerrainType.WATER, "~", Colors.Blue(), 1, 4);

        public static TerrainInfo Sand = new TerrainInfo(TerrainType.SAND, "~", Colors.Sand(), 1, 4);

        public static TerrainInfo Grass = new TerrainInfo(TerrainType.GRASS, ",", Colors.Green(), 0, -3);

        public static TerrainInfo Dirt = new TerrainInfo(TerrainType.DIRT, ".", Colors.Brown(), 0, -3);

        public static TerrainInfo Hill = new TerrainInfo(TerrainType.HILL, "∩", Colors.DarkGreen(), 0, 0);

        public static TerrainInfo Mountain = new TerrainInfo(TerrainType.MOUNTAIN, "▲", Colors.Grey(), 0, 0);
    }

    public struct TerrainInfo
    {
        public TerrainType Type { get; set; }

        public string Symbol { get; set; }
        public SDL.SDL_Color Color { get; set; }

        public int XMod { get; set; }
        public int YMod { get; set; }

        public TerrainInfo(TerrainType type, string symbol, SDL.SDL_Color color, int xMod, int yMod)
        {
            Type = type;
            Symbol = symbol;
            Color = color;
            XMod = xMod;
            YMod = yMod;
        }

        public bool Equals(TerrainInfo other)
        {
            return this.Symbol == other.Symbol &&
                   this.Color.r == other.Color.r &&
                   this.Color.g == other.Color.g &&
                   this.Color.b == other.Color.b &&
                   this.Color.a == other.Color.a &&
                   this.XMod == other.XMod &&
                   this.YMod == other.YMod;
        }
    }

    public enum TerrainType
    {
        NONE,
        WATER,
        SAND,
        GRASS,
        DIRT,
        HILL,
        MOUNTAIN,
    }
}
