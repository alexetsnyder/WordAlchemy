using WordAlchemy.Grids;

namespace WordAlchemy.WorldGen
{
    public class ChunkGen
    {
        public int ChunkRows { get; set; }
        public int ChunkCols { get; set; }

        public Point ChunkSize { get; set; }

        public ChunkGen(int chunkRows, int chunkCols, Point chunkSize)
        {
            ChunkRows = chunkRows;
            ChunkCols = chunkCols;

            ChunkSize = chunkSize;
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
            FillChunkGrid(mapChunk.Grid, terrainByte);
            mapChunk.GenerateChunkTexture();

            return mapChunk;
        }

        private void FillChunkGrid(BoundedGrid grid, byte terrainByte)
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
    }
}
