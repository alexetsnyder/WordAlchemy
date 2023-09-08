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

            int groupIndex = 0;
            foreach (var indexCellTuple in map.GetCells())
            {
                byte terrainByte = indexCellTuple.Item1;
                Cell cell = indexCellTuple.Item2;

                if (!IsCellGrouped(groupList, cell.I, cell.J))
                {
                    TerrainInfo terrainInfo = Terrain.TerrainArray[terrainByte];
                    TerrainType type = terrainInfo.Type;
                    Group group = new Group(groupIndex, type, type.ToString());

                    groupList.Add(group);

                    FillGroup(terrainByte, cell, group, groupList, map.GridCells);

                    groupIndex++;
                }
            }

            return groupList;
        }

        private bool IsCellGrouped(List<Group> groupList, int i, int j)
        {
            foreach (Group group in groupList)
            {
                if (group.IsCellInGroup(i, j))
                {
                    return true;
                }
            }
            return false;
        }

        private void FillGroup(byte terrainByte, Cell cell, Group group, List<Group> groupList, byte[] gridCells)
        {
            Stack<Tuple<int, int>> stack = new Stack<Tuple<int, int>>(new Tuple<int, int>[] { Tuple.Create(cell.I, cell.J) }); 
            
            while (stack.Count > 0)
            {
                Tuple<int, int> currentCell = stack.Pop();
                if (!IsCellGrouped(groupList, currentCell.Item1, currentCell.Item2))
                {
                    TerrainInfo terrainInfo = Terrain.TerrainArray[terrainByte];

                    group.CellDict.Add(Tuple.Create(currentCell.Item1, currentCell.Item2), terrainByte);

                    List<Tuple<int, int>> connectedCellList = GetConnectedCells(currentCell.Item1, currentCell.Item2);

                    foreach (var connectedCell in connectedCellList)
                    {
                        int i = connectedCell.Item1;
                        int j = connectedCell.Item2;

                        if (Terrain.TerrainArray[gridCells[i * Cols + j]].Equals(terrainInfo))
                        {
                            stack.Push(connectedCell);
                        }
                    }
                }
            } 
        }

        private List<Cell> GetConnectedCells(Grid grid, Cell cell)
        {
            return GetConnectedCells(cell.I, cell.J).Select(t => grid.GetCell(t.Item1, t.Item2)).ToList();
        }

        private List<Tuple<int, int>> GetConnectedCells(int i,  int j)
        {
            List<Tuple<int, int>> connectedCellIndexes = new List<Tuple<int, int>>()
            {
                new Tuple<int, int>(i - 1, j - 1),
                new Tuple<int, int>(i - 1, j    ),
                new Tuple<int, int>(i - 1, j + 1),
                new Tuple<int, int>(i,     j - 1),
                new Tuple<int, int>(i,     j + 1),
                new Tuple<int, int>(i + 1, j - 1),
                new Tuple<int, int>(i + 1, j    ),
                new Tuple<int, int>(i + 1, j + 1),
            };

            List<Tuple<int, int>> connectedCells = new List<Tuple<int, int>>();
            foreach (var tuple in connectedCellIndexes)
            {
                if (tuple.Item1 >= 0 && tuple.Item1 < Rows &&
                    tuple.Item2 >= 0 && tuple.Item2 < Cols)
                {
                    connectedCells.Add(tuple);
                }
            }

            return connectedCells;
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
                    List<Cell> cellList = group.CellDict.Keys.Select(t => map.Grid.GetCell(t.Item1, t.Item2)).ToList();
                    Cell? cell = GetMaxHeight(cellList);

                    if (cell.HasValue)
                    {
                        Func<Cell, Cell, bool> StartCellCheck = GetStartCellCheck(cell.Value);
                        Func<Cell, Cell, bool> OrCellCheck = GetOrCellCheck(cell.Value);

                        Cell? startCell = GetStartCell(map.Grid, cell.Value, StartCellCheck);

                        if (startCell.HasValue)
                        {
                            Group riverGroup = new Group(nextGroupId++, TerrainType.RIVER, TerrainType.RIVER.ToString());

                            GenerateRiverRecursive(riverGroup, startCell.Value, map, OrCellCheck);

                            riverGroupList.Add(riverGroup);
                        }             
                    } 
                }
            }

            if (riverGroupList.Count > 0)
            {
                foreach (Group riverGroup in riverGroupList)
                {
                    foreach (Tuple<int, int> key in riverGroup.CellDict.Keys)
                    {
                        foreach (Group group in map.GroupList)
                        {
                            if (group.IsCellInGroup(key.Item1, key.Item2))
                            {
                                group.CellDict.Remove(key);
                            }
                        }
                    }
                }

                map.GroupList.AddRange(riverGroupList);
            }
        }

        private Func<Cell, Cell, bool> GetStartCellCheck(Cell cell)
        {
            Func<Cell, Cell, bool> StartCellCheck;

            if (cell.X <= Width / 2 && cell.Y <= Height / 2)
            {
                StartCellCheck = (c1, c2) => c1.Y == c2.Y && c1.X > c2.X;
            }
            else if (cell.X > Width / 2 && cell.Y <= Height / 2)
            {
                StartCellCheck = (c1, c2) => c1.Y == c2.Y && c1.X < c2.X;
            }
            else if (cell.X <= Width / 2 && cell.Y > Height / 2)
            {
                StartCellCheck = (c1, c2) => c1.Y == c2.Y && c1.X > c2.X;
            }
            else // Cell.X > Width / 2 && Cell.Y > Height / 2
            {
                StartCellCheck = (c1, c2) => c1.Y == c2.Y && c1.X < c2.X;
            }

            return StartCellCheck;
        }

        private Func<Cell, Cell, bool> GetOrCellCheck(Cell cell)
        {
            Func<Cell, Cell, bool> OrCellCheck;

            if (cell.X <= Width / 2 && cell.Y <= Height / 2)
            {
                OrCellCheck = (c1, c2) => c1.Y <= c2.Y && c1.X >= c2.X;
            }
            else if (cell.X > Width / 2 && cell.Y <= Height / 2)
            {
                OrCellCheck = (c1, c2) => c1.Y <= c2.Y && c1.X <= c2.X;
            }
            else if (cell.X <= Width / 2 && cell.Y > Height / 2)
            {
                OrCellCheck = (c1, c2) => c1.Y >= c2.Y && c1.X >= c2.X;
            }
            else // Cell.X > Width / 2 && Cell.Y > Height / 2
            {
                OrCellCheck = (c1, c2) => c1.Y >= c2.Y && c1.X <= c2.X;
            }

            return OrCellCheck;
        }

        private Cell? GetStartCell(Grid grid, Cell cell, Func<Cell, Cell, bool> StartCellCheck)
        {
            List<Cell> connectedCellList = GetConnectedCells(grid, cell);

            foreach (Cell connectedCell in connectedCellList)
            {
                if (StartCellCheck(connectedCell, cell))
                {
                    return connectedCell;
                }
            }

            return null;
        }

        private void GenerateRiverRecursive(Group group, Cell cell, Map map, Func<Cell, Cell, bool> OrCellCheck)
        {
            byte terrainByte = map.GridCells[cell.I * Cols + cell.J];
            TerrainType type = Terrain.TerrainArray[terrainByte].Type;

            if (type != TerrainType.WATER)
            {
                map.GridCells[cell.I * Cols + cell.J] = 0;
                group.CellDict.Add(Tuple.Create(cell.I, cell.J), 0);

                List<Cell> connectedCellList = GetConnectedCells(map.Grid, cell);

                List<Cell> possibleCells = new List<Cell>();
                foreach (Cell connectedCell in connectedCellList)
                {
                    if (OrCellCheck(connectedCell, cell) && group.IsCellInGroup(cell.I, cell.J))
                    {
                        possibleCells.Add(connectedCell);
                    }
                }

                Cell? minCell = GetMinHeight(possibleCells);
                if (minCell.HasValue)
                {
                    GenerateRiverRecursive(group, minCell.Value, map, OrCellCheck);
                }
            }
        }

        private Cell? GetMinHeight(List<Cell> cellList)
        {
            float minHeight = float.MaxValue;
            Cell? minCell = null;

            foreach (Cell cell in cellList)
            {
                float height = HeightMap[cell.I * Cols + cell.J];

                if (height < minHeight)
                {
                    minHeight = height;
                    minCell = cell;
                }
            }

            return minCell;
        }

        private Cell? GetMaxHeight(List<Cell> cellList)
        {
            float maxHeight = float.MinValue;
            Cell? maxCell = null;

            foreach (Cell cell in cellList)
            {
                float heightValue = HeightMap[cell.I * Cols + cell.J];

                if (heightValue > maxHeight)
                {
                    maxHeight = heightValue;
                    maxCell = cell;
                }
            }

            return maxCell;
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
