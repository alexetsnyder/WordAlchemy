
using SDL2;
using System.Diagnostics;
using WordAlchemy.WorldGen;

namespace WordAlchemy.Viewers
{
    public class MapViewer : Viewer
    {
        public Map Map { get; set; }

        public SDL.SDL_Rect? SelectRect { get; private set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public MapViewer(Map map, ViewWindow? srcViewWindow = null, ViewWindow? dstViewWindow = null)
            : base(srcViewWindow, dstViewWindow)
        {
            Map = map;

            SelectRect = null;
            KeysPressedList = new List<SDL.SDL_Keycode>();

            GraphicSystem = GraphicSystem.Instance;

            WireEvents();
        }

        private void WireEvents()
        {
            EventSystem eventSystem = EventSystem.Instance;
            eventSystem.Listen((int)GameState.MAP, SDL.SDL_EventType.SDL_KEYDOWN, OnKeyDown);
            eventSystem.Listen((int)GameState.MAP, SDL.SDL_EventType.SDL_KEYUP, OnKeyUp);
            eventSystem.Listen((int)GameState.MAP, SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN, OnMouseButtonDown);
        }

        public void Update()
        {
            HandleKeys();
            CheckSelection();
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

                GraphicSystem.SetDrawColor(Colors.Red());
                GraphicSystem.DrawRect(ref rect);
            }

            if (DstViewWindow != null && SrcViewWindow != null)
            {
                SDL.SDL_Rect src = SrcViewWindow.GetViewRect();
                SDL.SDL_Rect dest = DstViewWindow.GetViewRect();

                Map.Draw(ref src, ref dest);
            }
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

        private void CheckSelection()
        {
            if (SelectRect.HasValue && Map.CurrentMapNode != null)
            {
                if (SelectRect.Value.x != Map.CurrentMapNode.X || SelectRect.Value.y != Map.CurrentMapNode.Y)
                {
                    SelectRect = new SDL.SDL_Rect
                    {
                        x = Map.CurrentMapNode.X,
                        y = Map.CurrentMapNode.Y,
                        w = Map.MapGen.CharWidth,
                        h = Map.MapGen.CharHeight,
                    };
                }
            }
        }

        private int GetXMax()
        {
            int textureWidth = Map.MapGen.Width;

            if (SrcViewWindow == null || textureWidth <= SrcViewWindow.Width)
            {
                return 0;
            }
            return textureWidth - SrcViewWindow.Width;
        }

        private int GetYMax()
        {
            int textureHeight = Map.MapGen.Height;

            if (SrcViewWindow == null || textureHeight <= SrcViewWindow.Height)
            {
                return 0;
            }
            return textureHeight - SrcViewWindow.Height;
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

                MapNode? mapNode = Map.GetMapNode(worldX, worldY);

                if (mapNode != null)
                {
                    Map.CurrentMapNode = mapNode;

                    SelectRect = new SDL.SDL_Rect
                    {
                        x = mapNode.X,
                        y = mapNode.Y,
                        w = Map.MapGen.CharWidth,
                        h = Map.MapGen.CharHeight,
                    };
                }
            }
        }
    }
}
