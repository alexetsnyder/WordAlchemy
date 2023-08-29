
using SDL2;
using System.Diagnostics;
using WordAlchemy.Helpers;

namespace WordAlchemy
{
    internal class Map
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int Rows { get; set; }
        public int Cols { get; set; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public ViewWindow ViewWindow { get; set; }

        public Graph Graph { get; set; }

        public MapGen MapGen { get; set; }

        public IntPtr MapTexture { get; set; }

        private SDL.SDL_Rect? SelectRect { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private static readonly int CharWidth = 8;
        private static readonly int CharHeight = 14;

        private SDLGraphics Graphics { get; set; }

        public Map(int width, int height, int rows, int cols, int offsetX = 0, int offsetY = 0)
        {
            Width = width;
            Height = height;

            Rows = rows; 
            Cols = cols; 

            OffsetX = offsetX;
            OffsetY = offsetY;

            ViewWindow = new ViewWindow(0, 0, width, height);

            Graph = new Graph();

            Random random = new Random();
            MapGen = new MapGen(Rows, Cols, random.Next(0, 1000000));
            MapGen.GenerateMap();

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

        public void GenerateMap()
        {
            for (int i = 0; i < Rows;  i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    int x = j * CharWidth;
                    int y = i * CharHeight;

                    TerrainInfo terrain = MapGen.GetTerrain(i, j);

                    MapNode mapNode = new MapNode(i * Cols + j, x, y, terrain);

                    Graph.AddNode(mapNode);
                    if (j != 0)
                    {
                        Edge newEdge = new Edge(Graph.NodeList[i * Cols + (j - 1)], mapNode);
                        Graph.AddEdge(newEdge);
                    }
                    if (i != 0)
                    {
                        Edge newEdge = new Edge(Graph.NodeList[(i - 1) * Cols + j], mapNode);
                        Graph.AddEdge(newEdge);
                    }                 
                }
            }

            GenerateMapTexture();
        }

        public void GenerateMapTexture()
        {
            MapTexture = Graphics.CreateTexture(Cols * CharWidth, Rows * CharHeight);

            foreach (MapNode mapNode in Graph.NodeList)
            {
                mapNode.DrawTo(MapTexture);
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

            SDL.SDL_Rect src = ViewWindow.GetViewRect();
            
            SDL.SDL_Rect dest = new SDL.SDL_Rect
            {
                x = OffsetX,
                y = OffsetY,
                w = Width,
                h = Height,
            };

            Graphics.DrawTexture(MapTexture, ref src, ref dest);
        }

        private void HandleKeys()
        {
            int speed = 5;

            foreach (var key in KeysPressedList)
            {
                if (key == SDL.SDL_Keycode.SDLK_w)
                {
                    ViewWindow.OffsetY -= speed;
                    ViewWindow.OffsetY = Math.Clamp(ViewWindow.OffsetY, 0, GetYMax());
                }
                if (key == SDL.SDL_Keycode.SDLK_s)
                {
                    ViewWindow.OffsetY += speed;
                    ViewWindow.OffsetY = Math.Clamp(ViewWindow.OffsetY, 0, GetYMax());
                }
                if (key == SDL.SDL_Keycode.SDLK_a)
                {
                    ViewWindow.OffsetX -= speed;
                    ViewWindow.OffsetX = Math.Clamp(ViewWindow.OffsetX, 0, GetXMax());
                }
                if (key == SDL.SDL_Keycode.SDLK_d)
                {
                    ViewWindow.OffsetX += speed;
                    ViewWindow.OffsetX = Math.Clamp(ViewWindow.OffsetX, 0, GetXMax());
                }
            }
        }

        private int GetXMax()
        {
            int textureWidth = Cols * CharWidth;

            if (textureWidth <= Width)
            {
                return 0;
            }
            return textureWidth - Width;
        }

        private int GetYMax()
        {
            int textureHeight = Rows * CharHeight;

            if (textureHeight <= Height)
            {
                return 0;
            }
            return textureHeight - Height;
        }

        public void ScreenToWorld(int screenX, int screenY, out int worldX, out int worldY)
        {
            worldX = screenX - OffsetX + ViewWindow.OffsetX;
            worldY = screenY - OffsetY + ViewWindow.OffsetY;
        }

        public void WorldToScreen(int worldX, int worldY, out int screenX, out int screenY)
        {
            screenX = worldX + OffsetX - ViewWindow.OffsetX;
            screenY = worldY + OffsetY - ViewWindow.OffsetY;
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
            if (e.button.button == SDL.SDL_BUTTON_LEFT)
            {
                SDL.SDL_GetMouseState(out int screenX, out int screenY);
                Debug.WriteLine($"Mouse X: {screenX}, Mouse Y: {screenY}");

                ScreenToWorld(screenX, screenY, out int worldX, out int worldY);

                foreach (MapNode mapNode in Graph.NodeList)
                {
                    int Ax = mapNode.X, Ay = mapNode.Y;
                    int Bx = Ax + CharWidth, By = Ay;
                    int Cx = Ax, Cy = Ay + CharHeight;

                    if (MathHelper.IsInRectangle(Ax, Ay, Bx, By, Cx, Cy, worldX, worldY))
                    {
                        SelectRect = new SDL.SDL_Rect
                        {
                            x = mapNode.X,
                            y = mapNode.Y,
                            w = CharWidth,
                            h = CharHeight,
                        };
                    }
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
