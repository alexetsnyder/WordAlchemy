
using SDL2;
using WordAlchemy.Helpers;

namespace WordAlchemy.WorldGen
{
    public class World
    {
        public int TopLeftX { get; set; }
        public int TopLeftY { get; set; }

        public Map Map { get; set; }

        public MapChunk? CenterChunk { get; private set; }

        private List<MapChunk> ChunksInView { get; set; }

        private Dictionary<Tuple<int, int>, MapChunk> AllGeneratedChunks { get; set; }

        public World(Map map)
        {
            Map = map;

            TopLeftX = 0;
            TopLeftY = 0;

            CenterChunk = null;
            ChunksInView = new List<MapChunk>();
            AllGeneratedChunks = new Dictionary<Tuple<int, int>, MapChunk>();
        }

        public bool IsChunkAlreadyGenerated(int chunkX, int chunkY)
        {
            if (AllGeneratedChunks.ContainsKey(Tuple.Create(chunkX, chunkY)))
            {
                return true;
            }
            return false;
        }

        public void SetCenterChunk(MapChunk mapChunk)
        {
            CenterChunk = mapChunk;
        }

        public void SetTopLeft(int x, int y)
        {
            int windowWidth = AppSettings.Instance.WindowWidth;
            int windowHeight = AppSettings.Instance.WindowHeight;

            TopLeftX = x - windowWidth / 2 + Map.MapGen.ChunkWidth / 2;
            TopLeftY = y - windowHeight / 2 + Map.MapGen.ChunkHeight / 2;
        }

        public void CopyChunkToView(int chunkX, int chunkY)
        {
            if (IsChunkAlreadyGenerated(chunkX, chunkY))
            {
                ChunksInView.Add(AllGeneratedChunks[Tuple.Create(chunkX, chunkY)]);
            }
        }

        public void AddChunkToView(MapChunk chunk)
        {
            if (!AllGeneratedChunks.ContainsKey(Tuple.Create(chunk.X, chunk.Y)))
            {
                ChunksInView.Add(chunk);
                AllGeneratedChunks.Add(Tuple.Create(chunk.X, chunk.Y), chunk);
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

        public void CalculateChunksInView(int worldX, int worldY)
        {
            MapChunk? mapChunk = GetMapChunkFromWorld(worldX, worldY);
            if (mapChunk != null && mapChunk != CenterChunk)
            {
                Cell oldCell = Map.MapGen.ChunkToMapCell(Map.Grid, CenterChunk.X, CenterChunk.Y);
                SetCenterChunk(mapChunk);
                Cell newCell = Map.MapGen.ChunkToMapCell(Map.Grid, mapChunk.X, mapChunk.Y);
                Map.MapGen.RegenerateWorld(this, Map, newCell, false);
                Map.SelectedCell = newCell;
            }
        }

        public void Draw(ref SDL.SDL_Rect src, ref SDL.SDL_Rect dst)
        {
            foreach (MapChunk mapChunk in ChunksInView)
            {
                dst.x = mapChunk.X - TopLeftX;
                dst.y = mapChunk.Y - TopLeftY;

                mapChunk.Draw(ref src, ref dst);

                GraphicSystem.Instance.SetDrawColor(Colors.Red());
                GraphicSystem.Instance.DrawRect(ref dst);
            }
        }

        public MapChunk? GetMapChunk(int chunkX, int chunkY)
        {
            Tuple<int, int> chunkCoord = Tuple.Create(chunkX, chunkY);
            
            if (AllGeneratedChunks.ContainsKey(chunkCoord))
            {
                return AllGeneratedChunks[chunkCoord];
            }

            return null;
        }

        public MapChunk? GetMapChunkFromWorld(int worldX, int worldY)
        {
            foreach (MapChunk chunk in ChunksInView)
            {
                int Ax = chunk.X; int Ay = chunk.Y;
                int Bx = chunk.X + chunk.Width; int By = chunk.Y;
                int Cx = chunk.X + chunk.Width; int Cy = chunk.Y + chunk.Height;

                if (MathHelper.IsInRectangle(Ax, Ay, Bx, By, Cx, Cy, worldX, worldY))
                {
                    return chunk;
                }
            }

            return null;
        }
    }
}
