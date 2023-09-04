
using SDL2;
using WordAlchemy.WorldGen;

namespace WordAlchemy
{
    public class PlayerViewer : Viewer
    {
        public Player Player { get; set; }

        public MapChunk? MapChunk { get; set; }

        public List<MapChunk> ChunkList { get; set; }

        public PlayerViewer(ViewWindow? srcViewWindow, ViewWindow? dstViewWindow)
            : base(srcViewWindow,  dstViewWindow)
        {
            Player = new Player();
            MapChunk = null;
            ChunkList = new List<MapChunk>();
        }

        public void Update()
        {

        }

        public void Draw()
        {
            if (MapChunk != null && SrcViewWindow != null && DstViewWindow != null)
            {
                SDL.SDL_Rect src = SrcViewWindow.GetViewRect();
                SDL.SDL_Rect dst = DstViewWindow.GetViewRect();

                MapChunk.Draw(ref src, ref dst);
            }
        }
    }
}
