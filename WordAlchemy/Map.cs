
using SDL2;

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

        public List<MapNode> MapNodeList { get; set; }

        public MapGen MapGen { get; set; }

        public IntPtr MapTexture { get; set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private static readonly int CharWidth = 8;
        private static readonly int CharHeight = 16;

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
            MapNodeList = new List<MapNode>();

            Random random = new Random();
            MapGen = new MapGen(Rows, Cols, random.Next(0, 1000000));
            MapGen.GenerateMap();

            MapTexture = IntPtr.Zero;

            KeysPressedList = new List<SDL.SDL_Keycode>();

            Graphics = SDLGraphics.Instance;
            WireEvents();
        }

        private void WireEvents()
        {
            EventSystem eventSystem = EventSystem.Instance;
            eventSystem.Listen(SDL.SDL_EventType.SDL_KEYDOWN, OnKeyDown);
            eventSystem.Listen(SDL.SDL_EventType.SDL_KEYUP, OnKeyUp);
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

                    Node node = new Node(i * Cols + j, x, y);

                    Graph.AddNode(node);
                    if (j != 0)
                    {
                        Edge newEdge = new Edge(MapNodeList[i * Cols + (j - 1)].Node, node);
                        Graph.AddEdge(newEdge);
                    }
                    if (i != 0)
                    {
                        Edge newEdge = new Edge(MapNodeList[(i - 1) * Cols + j].Node, node);
                        Graph.AddEdge(newEdge);
                    }                 

                    MapNode mapNode = new MapNode(node, terrain);
                    node.Reference = mapNode;

                    MapNodeList.Add(mapNode);
                }
            }

            GenerateMapTexture();
        }

        public void GenerateMapTexture()
        {
            MapTexture = Graphics.CreateTexture(Cols * CharWidth, Rows * CharHeight);

            foreach (MapNode mapNode in MapNodeList)
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

        public int GetXMax()
        {
            int textureWidth = Cols * CharWidth;

            if (textureWidth <= Width)
            {
                return 0;
            }
            return textureWidth - Width;
        }

        public int GetYMax()
        {
            int textureHeight = Rows * CharHeight;

            if (textureHeight <= Height)
            {
                return 0;
            }
            return textureHeight - Height;
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
