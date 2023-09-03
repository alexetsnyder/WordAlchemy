
using SDL2;

namespace WordAlchemy.WorldGen
{
    public class MapChunk
    {
        public MapNode MapNode { get; private set; }

        public Graph ChunkGraph { get; private set; }

        private IntPtr MapTexture { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public MapChunk(MapNode mapNode, Graph chunkGraph)
        {
            MapNode = mapNode;
            ChunkGraph = chunkGraph;
            MapTexture = IntPtr.Zero;

            GraphicSystem = GraphicSystem.Instance;
        }

        public void GenerateChunkTexture(int width, int height)
        {
            MapTexture = GraphicSystem.CreateTexture(width, height);

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
