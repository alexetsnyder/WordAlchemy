
using SDL2;

namespace WordAlchemy
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

        public void ScreenToWorld(int screenX, int screenY, out int worldX, out int worldY)
        {
            if (DstViewWindow == null || SrcViewWindow == null)
            {
                worldX = screenX;
                worldY = screenY;
                return;
            }

            worldX = screenX - DstViewWindow.OffsetX + SrcViewWindow.OffsetX;
            worldY = screenY - DstViewWindow.OffsetY + SrcViewWindow.OffsetY;
        }

        public void WorldToScreen(int worldX, int worldY, out int screenX, out int screenY)
        {
            if (DstViewWindow == null || SrcViewWindow == null)
            {
                screenX = worldX;
                screenY = worldY;
                return;
            }

            screenX = worldX + DstViewWindow.OffsetX - SrcViewWindow.OffsetX;
            screenY = worldY + DstViewWindow.OffsetY - SrcViewWindow.OffsetY;
        }
    }

    public class ViewWindow
    {
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public ViewWindow(int offsetX, int offsetY, int width, int height)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Width = width;
            Height = height;
        }

        public SDL.SDL_Rect GetViewRect()
        {
            return new SDL.SDL_Rect
            {
                x = OffsetX,
                y = OffsetY,
                w = Width,
                h = Height,
            };
        }
    }
}
