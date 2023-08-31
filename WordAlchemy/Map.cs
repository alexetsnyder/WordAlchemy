
using SDL2;
using System.Diagnostics;
using WordAlchemy.Helpers;

namespace WordAlchemy
{
    internal class Map
    {
        public ViewWindow? SrcViewWindow { get; set; }

        public ViewWindow? DstViewWindow { get; set; }

        public Graph? Graph { get; set; }

        public List<Group> GroupList { get; set; }

        public MapGen MapGen { get; set; }

        public IntPtr MapTexture { get; set; }

        private SDL.SDL_Rect? SelectRect { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private SDLGraphics Graphics { get; set; }

        public Map(MapGen mapGen, int rows, int cols)
        {
            DstViewWindow = null;
            SrcViewWindow = null;

            MapGen = mapGen;

            Graph = null;
            GroupList = new List<Group>();

            MapTexture = IntPtr.Zero;

            SelectRect = null;

            KeysPressedList = new List<SDL.SDL_Keycode>();

            Graphics = SDLGraphics.Instance;
            WireEvents();
        }

        private void WireEvents()
        {
            EventSystem eventSystem = EventSystem.Instance;
            eventSystem.Listen(SDL.SDL_EventType.SDL_KEYDOWN, OnKeyDown);
            eventSystem.Listen(SDL.SDL_EventType.SDL_KEYUP, OnKeyUp);
            eventSystem.Listen(SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN, OnMouseButtonDown);
        }

        public void GenerateMapTexture()
        {
            MapTexture = Graphics.CreateTexture(MapGen.Width, MapGen.Height);

            if (Graph != null)
            {
                foreach (MapNode mapNode in Graph.NodeList)
                {
                    mapNode.DrawTo(MapTexture);
                }
            } 
        }

        public void Update()
        {
            HandleKeys();
        }

        public void Draw()
        {
            if (SelectRect.HasValue)
            {
                WorldToScreen(SelectRect.Value.x, SelectRect.Value.y, out int x, out int y);

                SDL.SDL_Rect rect = new SDL.SDL_Rect
                {
                    x = x,
                    y = y,
                    w = SelectRect.Value.w,
                    h = SelectRect.Value.h,
                };

                Graphics.SetDrawColor(Colors.Red());
                Graphics.DrawRect(ref rect);
            }

            if (DstViewWindow != null && SrcViewWindow != null)
            {
                SDL.SDL_Rect src = SrcViewWindow.GetViewRect();
                SDL.SDL_Rect dest = DstViewWindow.GetViewRect();

                Graphics.DrawTexture(MapTexture, ref src, ref dest);
            }  
        }

        public Group? GetGroup(int groupId)
        {
            foreach (Group group in GroupList)
            {
                if (group.Id == groupId)
                {
                    return group;
                }
            }

            return null;
        }

        public MapNode? GetMapNode(int worldX, int worldY)
        {
            if (Graph != null)
            {
                foreach (MapNode mapNode in Graph.NodeList)
                {
                    int Ax = mapNode.X, Ay = mapNode.Y;
                    int Bx = Ax + MapGen.CharWidth, By = Ay;
                    int Cx = Ax, Cy = Ay + MapGen.CharHeight;

                    if (MathHelper.IsInRectangle(Ax, Ay, Bx, By, Cx, Cy, worldX, worldY))
                    {
                        return mapNode;
                    }
                }
            }

            return null;
        }

        private void HandleKeys()
        {
            if (SrcViewWindow == null)
            {
                return;
            }

            int speed = 5;

            foreach (var key in KeysPressedList)
            {
                if (key == InputSettings.Instance.MapUp)
                {
                    SrcViewWindow.OffsetY -= speed;
                    SrcViewWindow.OffsetY = Math.Clamp(SrcViewWindow.OffsetY, 0, GetYMax());
                }
                if (key == InputSettings.Instance.MapDown)
                {
                    SrcViewWindow.OffsetY += speed;
                    SrcViewWindow.OffsetY = Math.Clamp(SrcViewWindow.OffsetY, 0, GetYMax());
                }
                if (key == InputSettings.Instance.MapLeft)
                {
                    SrcViewWindow.OffsetX -= speed;
                    SrcViewWindow.OffsetX = Math.Clamp(SrcViewWindow.OffsetX, 0, GetXMax());
                }
                if (key == InputSettings.Instance.MapRight)
                {
                    SrcViewWindow.OffsetX += speed;
                    SrcViewWindow.OffsetX = Math.Clamp(SrcViewWindow.OffsetX, 0, GetXMax());
                }
            }
        }

        private int GetXMax()
        {
            int textureWidth = MapGen.Width;

            if (SrcViewWindow == null || textureWidth <= SrcViewWindow.Width)
            {
                return 0;
            }
            return textureWidth - SrcViewWindow.Width;
        }

        private int GetYMax()
        {
            int textureHeight = MapGen.Height;

            if (SrcViewWindow == null || textureHeight <= SrcViewWindow.Height)
            {
                return 0;
            }
            return textureHeight - SrcViewWindow.Height;
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

        public void OnKeyDown(SDL.SDL_Event e)
        {
            if (!KeysPressedList.Contains(e.key.keysym.sym))
            {
                KeysPressedList.Add(e.key.keysym.sym);
            }
        }

        public void OnKeyUp(SDL.SDL_Event e)
        {
            KeysPressedList.Remove(e.key.keysym.sym);
        }

        public void OnMouseButtonDown(SDL.SDL_Event e)
        {
            if (e.button.button == InputSettings.Instance.MouseButtonSelect)
            {
                SDL.SDL_GetMouseState(out int screenX, out int screenY);
                Debug.WriteLine($"Mouse X: {screenX}, Mouse Y: {screenY}");

                ScreenToWorld(screenX, screenY, out int worldX, out int worldY);

                MapNode? mapNode = GetMapNode(worldX, worldY);

                if (mapNode != null)
                {
                    SelectRect = new SDL.SDL_Rect
                    {
                        x = mapNode.X,
                        y = mapNode.Y,
                        w = MapGen.CharWidth,
                        h = MapGen.CharHeight,
                    };
                }
            }
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
