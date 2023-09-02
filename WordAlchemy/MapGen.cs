
using WordAlchemy.Helpers;

namespace WordAlchemy
{
    public class MapGen
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

            SDLGraphics.Instance.SizeText(Terrain.Water.Symbol, AppSettings.Instance.MapFontName, out int width, out int height);
            CharWidth = width;
            CharHeight = height;

            HeightMap = new float[Cols * Rows];
        }

        public Map GenerateMap()
        {
            GenerateHeightMap();

            Map map = new Map(this);
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
                    AddEdges(graph, mapNode, i, j);
                }
            }
            return graph;
        }

        private void AddEdges(Graph graph, MapNode mapNode, int i, int j)
        {
            List<int[]> neighbors = new List<int[]>()
                    {
                        new int[] {i - 1, j - 1},
                        new int[] {i - 1, j    },
                        new int[] {i - 1, j + 1},
                        new int[] {i,     j - 1},
                        new int[] {i + 1, j - 1},
                    };

            foreach (int[] pair in neighbors)
            {
                MapNode? prevMapNode = GetMapNode(graph, pair[0], pair[1]);
                if (prevMapNode != null)
                {
                    Edge newEdge = new Edge(prevMapNode, mapNode);
                    graph.AddEdge(newEdge);
                }
            }
        }

        private MapNode? GetMapNode(Graph graph, int i, int j)
        {
            MapNode? mapNode = null;

            if (i >= 0 && i < Rows && j >= 0 && j < Cols)
            {
                int index = i * Cols + j;
                if (index < graph.NodeList.Count)
                {
                    mapNode = graph.NodeList[index] as MapNode;
                }    
            }

            return mapNode;
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
            int nextGroupId = map.GroupList.Last().Id + 1;
            List<Group> riverGroupList = new List<Group>();  
            foreach (Group group in map.GroupList)
            {
                if (group.Type == TerrainType.MOUNTAIN)
                {
                    MapNode mapNode = GetMaxHeight(group.MapNodeList);
                    Group riverGroup = new Group(nextGroupId++, TerrainType.WATER, TerrainType.WATER.ToString());

                    Func<MapNode, MapNode, bool> StartNodeCheck = GetStartNodeCheck(mapNode);
                    Func<MapNode, MapNode, bool> OrMapNodeCheck = GetOrMapNodeCheck(mapNode);

                    MapNode startMapNode = GetStartMapNode(mapNode, StartNodeCheck);
                    GenerateRiverRecursive(riverGroup, mapNode, OrMapNodeCheck);
                    
                    riverGroupList.Add(riverGroup);
                }
            }

            if (riverGroupList.Count > 0)
            {
                map.GroupList.AddRange(riverGroupList);
            }
        }

        private Func<MapNode, MapNode, bool> GetStartNodeCheck(MapNode mapNode)
        {
            Func<MapNode, MapNode, bool> StartNodeCheck;

            if (mapNode.X <= Width / 2 && mapNode.Y <= Height / 2)
            {
                StartNodeCheck = (m1, m2) => m1.Y == m2.Y && m1.X > m2.X;
            }
            else if (mapNode.X > Width / 2 && mapNode.Y <= Height / 2)
            {
                StartNodeCheck = (m1, m2) => m1.Y == m2.Y && m1.X < m2.X;
            }
            else if (mapNode.X <= Width / 2 && mapNode.Y > Height / 2)
            {
                StartNodeCheck = (m1, m2) => m1.Y == m2.Y && m1.X > m2.X;
            }
            else // mapNode.X > Width / 2 && mapNode.Y > Height / 2
            {
                StartNodeCheck = (m1, m2) => m1.Y == m2.Y && m1.X < m2.X;
            }

            return StartNodeCheck;
        }

        private Func<MapNode, MapNode, bool> GetOrMapNodeCheck(MapNode mapNode)
        {
            Func<MapNode, MapNode, bool> OrMapNodeCheck;

            if (mapNode.X <= Width / 2 && mapNode.Y <= Height / 2)
            {
                OrMapNodeCheck = (m1, m2) => m1.Y <= m2.Y && m1.X >= m2.X;
            }
            else if (mapNode.X > Width / 2 && mapNode.Y <= Height / 2)
            {
                OrMapNodeCheck = (m1, m2) => m1.Y <= m2.Y && m1.X <= m2.X;
            }
            else if (mapNode.X <= Width / 2 && mapNode.Y > Height / 2)
            {
                OrMapNodeCheck = (m1, m2) => m1.Y >= m2.Y && m1.X >= m2.X;
            }
            else // mapNode.X > Width / 2 && mapNode.Y > Height / 2
            {
                OrMapNodeCheck = (m1, m2) => m1.Y >= m2.Y && m1.X <= m2.X;
            }

            return OrMapNodeCheck;
        }

        private MapNode GetStartMapNode(MapNode mapNode, Func<MapNode, MapNode, bool> StartNodeCheck)
        {
            MapNode startingMapNode = mapNode;

            foreach (Edge edge in mapNode.EdgeList)
            {
                if (edge.V1 == mapNode)
                {
                    if (edge.V2 is MapNode node && StartNodeCheck(node, mapNode)) 
                    {
                        startingMapNode = node;
                        break;
                    }
                }
                else
                {
                    if (edge.V1 is MapNode node && StartNodeCheck(node, mapNode))
                    {
                        startingMapNode = node;
                        break;
                    }
                }    
            }

            return startingMapNode;
        }

        private void GenerateRiverRecursive(Group group, MapNode mapNode, Func<MapNode, MapNode, bool> OrMapNodeCheck)
        {
            TerrainType type = mapNode.Info.Type;
            if (type != TerrainType.WATER)
            {
                mapNode.Info = Terrain.Water;
                mapNode.GroupID = group.Id;
                group.MapNodeList.Add(mapNode);

                List<MapNode> possibleNodes = new List<MapNode>();
                foreach (Edge edge in mapNode.EdgeList)
                {
                    if (edge.V1 == mapNode)
                    {
                        if (edge.V2 is MapNode newNode && OrMapNodeCheck(newNode, mapNode))
                        {
                            if (newNode.GroupID != group.Id)
                            {
                                possibleNodes.Add(newNode);
                            }  
                        }
                    }
                    else
                    {
                        if (edge.V1 is MapNode newNode && OrMapNodeCheck(newNode, mapNode))
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
                    GenerateRiverRecursive(group, minMapNode, OrMapNodeCheck);
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
}
