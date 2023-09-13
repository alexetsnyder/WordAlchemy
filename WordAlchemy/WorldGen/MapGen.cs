using System.Diagnostics;
using WordAlchemy.Grids;
using WordAlchemy.Helpers;
using WordAlchemy.Settings;
using WordAlchemy.Systems;

namespace WordAlchemy.WorldGen
{
    public class MapGen
    {
        public int Width
        {
            get
            {
                return Cols * CharSize.W;
            }
        }

        public int Height
        {
            get
            {
                return Rows * CharSize.H;
            }
        }

        public int Rows { get; set; }
        public int Cols { get; set; }

        public int ChunkRows { get; set; }
        public int ChunkCols { get; set; }

        public Point ChunkSize { get; set; }

        public int Seed { get; set; }

        public Point CharSize { get; set; }

        private FastNoiseLite Noise { get; set; }
        private float[] HeightMap { get; set; }

        public MapGen(int rows, int cols, int chunkRows, int chunkCols, int seed)
        {
            Rows = rows;
            Cols = cols;
            ChunkRows = chunkRows;
            ChunkCols = chunkCols;

            Seed = seed;
            Noise = new FastNoiseLite(seed);
            Noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            Noise.SetFractalType(FastNoiseLite.FractalType.FBm);

            GraphicSystem.Instance.SizeText(Terrain.Water.Symbol, AppSettings.Instance.MapFontName, out int width, out int height);
            CharSize = new Point(width, height);

            ChunkSize = new Point(ChunkCols * CharSize.W, ChunkRows * CharSize.H);

            HeightMap = new float[Cols * Rows];
        }

        public Map GenerateMap()
        {
            GenerateHeightMap();

            Map map = new Map(this);
            FillGridCells(map.Grid);
            map.GroupList = GroupTerrain(map);

            ClassifyWaterGroups(map.GroupList);

            GenerateRivers(map);

            return map;
        }

        public void GenerateWorld(World world, Cell cell, bool isFullGeneration)
        {
            world.ClearChunksInView();

            GenerateWorldRecursive(world, cell, world.ViewDistance);

            Point chunkPos = new Point(cell.GridPos.J * ChunkSize.W, cell.GridPos.I * ChunkSize.H);

            MapChunk? centerChunk = world.GetMapChunk(chunkPos);
            if (centerChunk != null)
            {
                world.SetCenterChunk(centerChunk);
                if (isFullGeneration)
                {
                    world.SetTopLeft(centerChunk.ChunkPos);
                }
            }
        }

        public void GenerateWorldRecursive(World world, Cell cell, int viewDistance)
        {
            Point chunkPos = new Point(cell.GridPos.J * ChunkSize.W, cell.GridPos.I * ChunkSize.H);

            if (!world.IsChunkInView(chunkPos))
            {
                if (!world.IsChunkAlreadyGenerated(chunkPos))
                {
                    byte terrainByte = world.Map.Grid.GetCellValue(cell);
                    MapChunk mapChunk = GenerateMapChunk(world, cell, chunkPos, terrainByte);
                    world.AddChunkToView(mapChunk);
                }
                else
                {
                    world.CopyChunkToView(chunkPos);
                }

                if (viewDistance > 0)
                {
                    List<Cell> cellList = world.Map.Grid.GetConnectedCells(cell);

                    foreach (Cell connectedCell in cellList)
                    {
                        GenerateWorldRecursive(world, connectedCell, viewDistance - 1);
                    }
                } 
            }
        }

        public MapChunk GenerateMapChunk(World world, Cell cell, Point chunkPos, byte terrainByte)
        {
            MapChunk mapChunk = new MapChunk(cell, chunkPos, ChunkRows, ChunkCols, ChunkSize);
            FillChunkGridCells(mapChunk.Grid, terrainByte);
            mapChunk.GenerateChunkTexture();

            return mapChunk;
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

        private void FillGridCells(BoundedGrid grid)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    Point gridPos = new Point(i, j);
                    Cell? cell = grid.GetCell(gridPos);
                    if (cell.HasValue)
                    {
                        grid.SetCellValue(cell.Value, GetTerrainByte(i, j));
                    }
                }
            }
        }

        private void FillChunkGridCells(BoundedGrid grid, byte terrainByte)
        {
            for (int i = 0; i < ChunkRows; i++)
            {
                for (int j = 0; j < ChunkCols; j++)
                {
                    Point gridPos = new Point(i, j);
                    Cell? cell = grid.GetCell(gridPos);
                    if (cell.HasValue)
                    {
                        grid.SetCellValue(cell.Value, terrainByte);
                    }
                }
            }
        }

        private List<Group> GroupTerrain(Map map)
        {
            List<Group> groupList = new List<Group>();

            int groupIndex = 0;
            foreach (Cell cell in map.Grid.GetCells())
            {
                byte terrainByte = map.Grid.GetCellValue(cell);

                if (!IsCellGrouped(groupList, cell))
                {
                    TerrainInfo terrainInfo = Terrain.TerrainArray[terrainByte];
                    TerrainType type = terrainInfo.Type;
                    Group group = new Group(groupIndex, type, type.ToString());

                    groupList.Add(group);

                    FillGroup(group, cell, terrainByte, map.Grid, groupList);

                    groupIndex++;
                }
            }

            return groupList;
        }

        private bool IsCellGrouped(List<Group> groupList, Cell cell)
        {
            foreach (Group group in groupList)
            {
                if (group.IsCellInGroup(cell))
                {
                    return true;
                }
            }
            return false;
        }

        private void FillGroup(Group group, Cell cell, byte terrainByte, BoundedGrid grid, List<Group> groupList)
        {
            Stack<Cell> stack = new Stack<Cell>(new Cell[] { cell }); 
            
            while (stack.Count > 0)
            {
                Cell currentCell = stack.Pop();
                if (!IsCellGrouped(groupList, currentCell))
                {
                    TerrainInfo terrainInfo = Terrain.TerrainArray[terrainByte];

                    group.AddCell(currentCell, terrainByte);

                    List<Cell> connectedCellList = grid.GetConnectedCells(currentCell);

                    foreach (Cell connectedCell in connectedCellList)
                    {
                        if (Terrain.TerrainArray[grid.GetCellValue(connectedCell)].Equals(terrainInfo))
                        {
                            stack.Push(connectedCell);
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
                    List<Cell> cellList = group.GetGroupCells(map.Grid);
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
                    foreach (Cell cell in riverGroup.GetGroupCells(map.Grid))
                    {
                        foreach (Group group in map.GroupList)
                        {
                            if (group.IsCellInGroup(cell))
                            {
                                group.RemoveCell(cell);
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

            if (cell.WorldPos.X <= Width / 2 && cell.WorldPos.Y <= Height / 2)
            {
                StartCellCheck = (c1, c2) => c1.WorldPos.Y == c2.WorldPos.Y && c1.WorldPos.X > c2.WorldPos.X;
            }
            else if (cell.WorldPos.X > Width / 2 && cell.WorldPos.Y <= Height / 2)
            {
                StartCellCheck = (c1, c2) => c1.WorldPos.Y == c2.WorldPos.Y && c1.WorldPos.X < c2.WorldPos.X;
            }
            else if (cell.WorldPos.X <= Width / 2 && cell.WorldPos.Y > Height / 2)
            {
                StartCellCheck = (c1, c2) => c1.WorldPos.Y == c2.WorldPos.Y && c1.WorldPos.X > c2.WorldPos.X;
            }
            else // Cell.X > Width / 2 && Cell.Y > Height / 2
            {
                StartCellCheck = (c1, c2) => c1.WorldPos.Y == c2.WorldPos.Y && c1.WorldPos.X < c2.WorldPos.X;
            }

            return StartCellCheck;
        }

        private Func<Cell, Cell, bool> GetOrCellCheck(Cell cell)
        {
            Func<Cell, Cell, bool> OrCellCheck;

            if (cell.WorldPos.X <= Width / 2 && cell.WorldPos.Y <= Height / 2)
            {
                OrCellCheck = (c1, c2) => c1.WorldPos.Y <= c2.WorldPos.Y && c1.WorldPos.X >= c2.WorldPos.X;
            }
            else if (cell.WorldPos.X > Width / 2 && cell.WorldPos.Y <= Height / 2)
            {
                OrCellCheck = (c1, c2) => c1.WorldPos.Y <= c2.WorldPos.Y && c1.WorldPos.X <= c2.WorldPos.X;
            }
            else if (cell.WorldPos.X <= Width / 2 && cell.WorldPos.Y > Height / 2)
            {
                OrCellCheck = (c1, c2) => c1.WorldPos.Y >= c2.WorldPos.Y && c1.WorldPos.X >= c2.WorldPos.X;
            }
            else // Cell.X > Width / 2 && Cell.Y > Height / 2
            {
                OrCellCheck = (c1, c2) => c1.WorldPos.Y >= c2.WorldPos.Y && c1.WorldPos.X <= c2.WorldPos.X;
            }

            return OrCellCheck;
        }

        private Cell? GetStartCell(BoundedGrid grid, Cell cell, Func<Cell, Cell, bool> StartCellCheck)
        {
            List<Cell> connectedCellList = grid.GetConnectedCells(cell);

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
            byte terrainByte = map.Grid.GetCellValue(cell);
            TerrainType type = Terrain.TerrainArray[terrainByte].Type;

            if (type != TerrainType.WATER)
            {
                map.Grid.SetCellValue(cell, 0);
                group.AddCell(cell, 0);

                List<Cell> connectedCellList = map.Grid.GetConnectedCells(cell);

                List<Cell> possibleCells = new List<Cell>();
                foreach (Cell connectedCell in connectedCellList)
                {
                    if (OrCellCheck(connectedCell, cell) && !group.IsCellInGroup(connectedCell))
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
                float height = HeightMap[cell.GridPos.I * Cols + cell.GridPos.J];

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
                float heightValue = HeightMap[cell.GridPos.I * Cols + cell.GridPos.J];

                if (heightValue > maxHeight)
                {
                    maxHeight = heightValue;
                    maxCell = cell;
                }
            }

            return maxCell;
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
