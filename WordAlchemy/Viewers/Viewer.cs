
using SDL2;
using WordAlchemy.Grids;

namespace WordAlchemy.Viewers
{
    public class Viewer
    {
        public ViewWindow? SrcViewWindow { get; set; }

        public ViewWindow? DstViewWindow { get; set; }

        public Viewer(ViewWindow? srcViewWindow, ViewWindow? dstViewWindow)
        {
            SrcViewWindow = srcViewWindow;
            DstViewWindow = dstViewWindow;
        }

        public void ScreenToWorld(Point screenPos, out Point worldPos)
        {
            if (DstViewWindow == null || SrcViewWindow == null)
            {
                worldPos = screenPos;
                return;
            }

            worldPos = screenPos - DstViewWindow.Offset + SrcViewWindow.Offset;  
        }

        public void WorldToScreen(Point worldPos, out Point screenPos)
        {
            if (DstViewWindow == null || SrcViewWindow == null)
            {
                screenPos = worldPos;
                return;
            }

            screenPos = worldPos + DstViewWindow.Offset - SrcViewWindow.Offset;
        }
    }

    public class ViewWindow
    {
        public Point Offset { get; set; }

        public Point Size { get; set; }

        public ViewWindow(Point offset, Point size)
        {
            Offset = offset;
            Size = size;
        }

        public SDL.SDL_Rect GetViewRect()
        {
            return new SDL.SDL_Rect
            {
                x = Offset.X,
                y = Offset.Y,
                w = Size.W,
                h = Size.H,
            };
        }
    }
}
