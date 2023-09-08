
using WordAlchemy.Helpers;

namespace WordAlchemy.WorldGen
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

            GraphicSystem.Instance.SizeText(Terrain.Water.Symbol, AppSettings.Instance.MapFontName, out int width, out int height);
            CharWidth = width;
            CharHeight = height;

            HeightMap = new float[Cols * Rows];
        }

        public Map GenerateMap()
        {
            GenerateHeightMap();

            Map map = new Map(this);
            FillGridCells(map.GridCells);

            map.Graph = GenerateGraph();
            map.GroupList = GroupTerrain(map);

            ClassifyWaterGroups(map.GroupList);

            GenerateRivers(map);

            return map;
        }

        public List<MapChunk> GenerateMapChunks(MapNode mapNode, int rows, int cols)
        {
            List<MapChunk> chunkList = new List<MapChunk>();

            int chunkWidth = cols * CharWidth;
            int chunkHeight = rows * CharHeight;

            if (mapNode.MapChunk == null)
            {
                int i = mapNode.Y / CharHeight;
                int j = mapNode.X / CharWidth;

                Graph chunkGraph = GenerateChunkGraph(mapNode.Info, rows, cols);
                MapChunk mapChunk = new MapChunk(mapNode, chunkGraph, j * chunkWidth, i * chunkHeight, chunkWidth, chunkHeight);
                mapChunk.GenerateChunkTexture();
                mapNode.MapChunk = mapChunk;
            }
            chunkList.Add(mapNode.MapChunk);

            List<MapNode> connectedNodeList = mapNode.GetConnectedNodes();

            foreach (MapNode connectedNode in connectedNodeList)
            {
                if (connectedNode.MapChunk == null)
                {
                    int i = connectedNode.Y / CharHeight;
                    int j = connectedNode.X / CharWidth;

                    Graph chunkGraph = GenerateChunkGraph(connectedNode.Info, rows, cols);
                    MapChunk mapChunk = new MapChunk(connectedNode, chunkGraph, j * chunkWidth, i * chunkHeight, chunkWidth, chunkHeight);
                    mapChunk.GenerateChunkTexture();
                    connectedNode.MapChunk = mapChunk;
                }
                chunkList.Add(connectedNode.MapChunk);
            }

            return chunkList;
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

        private void FillGridCells(byte[] gridCells)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    int x = j * CharWidth;
                    int y = i * CharHeight;

                    gridCells[i * Cols + j] = GetTerrainByte(i, j);
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

        private Graph GenerateChunkGraph(TerrainInfo terrain, int rows, int cols)
        {
            Graph graph = new Graph();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int x = j * CharWidth;
                    int y = i * CharHeight;

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

                    List<MapNode> connectedNodeList = currentNode.GetConnectedNodes();

                    foreach (MapNode connectedNode in connectedNodeList)
                    {
                        if (connectedNode.Info.Equals(currentNode.Info))
                        {
                            stack.Push(connectedNode);
                        }
                    }
                }
            } 
        }

        private void ClassifyWaterGroups(List<Group> groupList)
        {
            Group oceanGroup = groupList.First();
            if (oceanGroup.Type == TerrainType.WATER)
            {
                oceanGroup.Type = TerrainType.OCEAN;
                oceanGroup.Name = TerrainType.OCEAN.ToString();
            }
            
            foreach (Group group in groupList.Where(g => g.Type == TerrainType.WATER))
            {
                TerrainType type = TerrainType.SEA;
                group.Type = type;
                group.Name = type.ToString();
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
                    Group riverGroup = new Group(nextGroupId++, TerrainType.RIVER, TerrainType.RIVER.ToString());

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
            List<MapNode> connectedNodeList = mapNode.GetConnectedNodes();

            foreach (MapNode connectedNode in connectedNodeList)
            {
                if (StartNodeCheck(connectedNode, mapNode))
                {
                    return connectedNode;
                }
            }

            return mapNode;
        }

        private void GenerateRiverRecursive(Group group, MapNode mapNode, Func<MapNode, MapNode, bool> OrMapNodeCheck)
        {
            TerrainType type = mapNode.Info.Type;
            if (type != TerrainType.WATER)
            {
                mapNode.Info = Terrain.Water;
                mapNode.GroupID = group.Id;
                group.MapNodeList.Add(mapNode);

                List<MapNode> connectedNodeList = mapNode.GetConnectedNodes();

                List<MapNode> possibleNodes = new List<MapNode>();
                foreach (MapNode connectedNode in connectedNodeList)
                {
                    if (OrMapNodeCheck(connectedNode, mapNode) && connectedNode.GroupID != group.Id)
                    {
                        possibleNodes.Add(connectedNode);
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

        public byte GetTerrainByte(int i, int j)
        {
            float heightValue = HeightMap[i * Cols + j];

            byte terrain;
            if (heightValue < 20.0f)
            {
                terrain = 0;
            }
            else if (heightValue < 25.0f)
            {
                terrain = 1;
            }
            else if (heightValue < 40.0f)
            {
                terrain = 2;
            }
            else if (heightValue < 60.0f)
            {
                terrain = 4;
            }
            else //(heightValue < 100.0f)
            {
                terrain = 5;
            }

            return terrain;
        }
    }
}
