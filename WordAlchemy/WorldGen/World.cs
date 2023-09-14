
using SDL2;
using WordAlchemy.Helpers;
using WordAlchemy.Settings;
using WordAlchemy.Systems;
using WordAlchemy.Grids;

namespace WordAlchemy.WorldGen
{
    public class World
    {
        public int ViewDistance { get; set; }

        public Point TopLeft { get; set; }

        public Map Map { get; set; }

        public ChunkGen ChunkGen { get; set; }

        public MapChunk? CenterChunk { get; private set; }

        private List<MapChunk> ChunksInView { get; set; }

        private Dictionary<Tuple<int, int>, MapChunk> AllGeneratedChunks { get; set; }

        public World(Map map, ChunkGen chunkGen, int viewDistance)
        {
            ViewDistance = viewDistance;
            Map = map;
            ChunkGen = chunkGen;

            TopLeft = new Point(0, 0);

            CenterChunk = null;
            ChunksInView = new List<MapChunk>();
            AllGeneratedChunks = new Dictionary<Tuple<int, int>, MapChunk>();   
        }

        public bool IsChunkInView(Point chunkPos)
        {
            return ChunksInView.Any(chunk => chunk.ChunkPos.X == chunkPos.X && chunk.ChunkPos.Y == chunkPos.Y);
        }

        public bool IsChunkAlreadyGenerated(Point chunkPos)
        {
            if (AllGeneratedChunks.ContainsKey(chunkPos.PointTuple))
            {
                return true;
            }
            return false;
        }

        public void SetCenterChunk(MapChunk mapChunk)
        {
            CenterChunk = mapChunk;
        }

        public void SetTopLeft(Point topLeft)
        {
            int windowWidth = AppSettings.Instance.WindowWidth;
            int windowHeight = AppSettings.Instance.WindowHeight;

            Point topLeftMod = new Point(windowWidth / 2 + ChunkGen.ChunkSize.W / 2, windowHeight / 2 + ChunkGen.ChunkSize.H / 2);

            int x = topLeft.X - windowWidth / 2 + ChunkGen.ChunkSize.W / 2;
            int y = topLeft.Y - windowHeight / 2 + ChunkGen.ChunkSize.H / 2;

            TopLeft = new Point(x, y);
        }

        public void CopyChunkToView(Point chunkPos)
        {
            if (IsChunkAlreadyGenerated(chunkPos))
            {
                ChunksInView.Add(AllGeneratedChunks[chunkPos.PointTuple]);
            }
        }

        public void AddChunkToView(MapChunk chunk)
        {
            if (!AllGeneratedChunks.ContainsKey(chunk.ChunkPos.PointTuple))
            {
                ChunksInView.Add(chunk);
                AllGeneratedChunks.Add(chunk.ChunkPos.PointTuple, chunk);
            }   
        }

        public void ClearChunksInView()
        {
            ChunksInView.Clear();
        }

        public void RemoveChunkFromView(MapChunk chunk)
        {
            ChunksInView.Remove(chunk);
        }

        public void CalculateChunksInView(Point worldPos)
        {
            MapChunk? mapChunk = GetMapChunkFromWorld(worldPos);
            if (mapChunk != null && mapChunk != CenterChunk)
            {
                SetCenterChunk(mapChunk);
                ChunkGen.GenerateWorld(Map, this, mapChunk.MapCell, false);
                Map.SelectedCell = mapChunk.MapCell;
            }
        }

        public void Draw(ref SDL.SDL_Rect src, ref SDL.SDL_Rect dst)
        {
            foreach (MapChunk mapChunk in ChunksInView)
            {
                Point dstPos = mapChunk.ChunkPos - TopLeft;
                dst.x = dstPos.X;
                dst.y = dstPos.Y;

                mapChunk.Draw(ref src, ref dst);

                GraphicSystem.Instance.SetDrawColor(Colors.Red());
                GraphicSystem.Instance.DrawRect(ref dst);
            }
        }

        public MapChunk? GetMapChunk(Point chunkPos)
        {
            Tuple<int, int> chunkCoord = chunkPos.PointTuple;
            
            if (AllGeneratedChunks.ContainsKey(chunkCoord))
            {
                return AllGeneratedChunks[chunkCoord];
            }

            return null;
        }

        public MapChunk? GetMapChunkFromWorld(Point worldPos)
        {
            foreach (MapChunk chunk in ChunksInView)
            {
                int Ax = chunk.ChunkPos.X; int Ay = chunk.ChunkPos.Y;
                int Bx = chunk.ChunkPos.X + chunk.ChunkSize.W; int By = chunk.ChunkPos.Y;
                int Cx = chunk.ChunkPos.X + chunk.ChunkSize.W; int Cy = chunk.ChunkPos.Y + chunk.ChunkSize.H;

                if (MathHelper.IsInRectangle(Ax, Ay, Bx, By, Cx, Cy, worldPos.X, worldPos.Y))
                {
                    return chunk;
                }
            }

            return null;
        }
    }
}
