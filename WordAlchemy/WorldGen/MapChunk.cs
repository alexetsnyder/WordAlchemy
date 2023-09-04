
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

        private IntPtr MapTexture { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public MapChunk(MapNode mapNode, Graph chunkGraph, int x, int y, int width, int height)
        {
            X = x;
            Y = y;

            Width = width;
            Height = height;

            MapNode = mapNode;
            ChunkGraph = chunkGraph;
            MapTexture = IntPtr.Zero;

            GraphicSystem = GraphicSystem.Instance;  
        }

        public void GenerateChunkTexture()
        {
            MapTexture = GraphicSystem.CreateTexture(Width, Height);

            foreach (MapNode mapNode in ChunkGraph.NodeList)
            {
                mapNode.DrawTo(MapTexture);
            }
        }

        public void Draw(ref SDL.SDL_Rect src, ref SDL.SDL_Rect dst)
        {
            GraphicSystem.DrawTexture(MapTexture, ref src, ref dst);
        }
    }
}
