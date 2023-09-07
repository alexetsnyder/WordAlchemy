
using SDL2;

namespace WordAlchemy.WorldGen
{
    public class MapChunk
    {
        public int X {  get; set; }
        public int Y { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public MapNode MapNode { get; private set; }

        public Graph ChunkGraph { get; private set; }

        private IntPtr ChunkTexture { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public MapChunk(MapNode mapNode, Graph chunkGraph, int x, int y, int width, int height)
        {
            X = x;
            Y = y;

            Width = width;
            Height = height;

            MapNode = mapNode;
            ChunkGraph = chunkGraph;

            ChunkTexture = IntPtr.Zero;

            GraphicSystem = GraphicSystem.Instance;  
        }

        public void GenerateChunkTexture()
        {
            IntPtr chunkSurface = GenerateChunkSurface();

            ChunkTexture = GraphicSystem.CreateTextureFromSurface(chunkSurface);

            SDL.SDL_FreeSurface(chunkSurface);
        }

        private IntPtr GenerateChunkSurface()
        {
            IntPtr chunkSurface = GraphicSystem.CreateSurface(Width, Height);

            foreach (MapNode mapNode in ChunkGraph.NodeList )
            {
                mapNode.DrawToSurface(chunkSurface);
            }

            return chunkSurface;
        }

        public void Draw(ref SDL.SDL_Rect src, ref SDL.SDL_Rect dst)
        {
            GraphicSystem.DrawTexture(ChunkTexture, ref src, ref dst);
        }
    }
}
