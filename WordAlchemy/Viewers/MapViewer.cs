
using SDL2;
using System.Diagnostics;
using WordAlchemy.Grids;
using WordAlchemy.Settings;
using WordAlchemy.Systems;
using WordAlchemy.WorldGen;

namespace WordAlchemy.Viewers
{
    public class MapViewer : Viewer
    {
        public World World { get; set; }

        private UI HUD { get; set; }

        public SDL.SDL_Rect? SelectRect { get; private set; }

        private List<SDL.SDL_Keycode> KeysPressedList { get; set; }

        private GraphicSystem GraphicSystem { get; set; }

        public MapViewer(World world, ViewWindow? srcViewWindow = null, ViewWindow? dstViewWindow = null)
            : base(srcViewWindow, dstViewWindow)
        {
            World = world;  
            HUD = new UI();

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
            UpdateUI();
        }

        public void Draw()
        {
            if (SelectRect.HasValue)
            {
                WorldToScreen(new Point(SelectRect.Value.x, SelectRect.Value.y), out Point screenPos);

                SDL.SDL_Rect rect = new SDL.SDL_Rect
                {
                    x = screenPos.X,
                    y = screenPos.Y,
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

                World.Map.Draw(ref src, ref dest);
            }

            HUD.Draw();
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
                    int y = Math.Clamp(SrcViewWindow.Offset.Y - speed, 0, GetYMax());
                    SrcViewWindow.Offset = new Point(SrcViewWindow.Offset.X, y);
                }
                if (key == InputSettings.Instance.MapDown)
                {
                    int y = Math.Clamp(SrcViewWindow.Offset.Y + speed, 0, GetYMax());
                    SrcViewWindow.Offset = new Point(SrcViewWindow.Offset.X, y);
                }
                if (key == InputSettings.Instance.MapLeft)
                {
                    int x = Math.Clamp(SrcViewWindow.Offset.X - speed, 0, GetXMax());
                    SrcViewWindow.Offset = new Point(x, SrcViewWindow.Offset.Y);
                }
                if (key == InputSettings.Instance.MapRight)
                {
                    int x = Math.Clamp(SrcViewWindow.Offset.X + speed, 0, GetXMax());
                    SrcViewWindow.Offset = new Point(x, SrcViewWindow.Offset.Y);
                }
            }
        }

        private void CheckSelection()
        {
            if (SelectRect.HasValue && World.Map.SelectedCell.HasValue)
            {
                if (SelectRect.Value.x != World.Map.SelectedCell.Value.WorldPos.X || SelectRect.Value.y != World.Map.SelectedCell.Value.WorldPos.Y)
                {
                    SelectRect = new SDL.SDL_Rect
                    {
                        x = World.Map.SelectedCell.Value.WorldPos.X,
                        y = World.Map.SelectedCell.Value.WorldPos.Y,
                        w = World.Map.Grid.CellSize.W,
                        h = World.Map.Grid.CellSize.H,
                    };
                }
            }
        }

        private void UpdateUI()
        {
            SDL.SDL_GetMouseState(out int screenX, out int screenY);

            ScreenToWorld(new Point(screenX, screenY), out Point worldPos);

            Cell? cell = World.Map.GetCell(worldPos);
            if (cell.HasValue && World.Map.IsCellGrouped(cell.Value))
            {
                Group? group = World.Map.GetGroup(cell.Value);
                if (group != null)
                {
                    HUD.SetGroupTypeStr($"{group.Name} {group.Id}");
                }
            }
        }

        private int GetXMax()
        {
            int textureWidth = World.Map.Grid.Size.W;

            if (SrcViewWindow == null || textureWidth <= SrcViewWindow.Size.W)
            {
                return 0;
            }
            return textureWidth - SrcViewWindow.Size.W;
        }

        private int GetYMax()
        {
            int textureHeight = World.Map.Grid.Size.H;

            if (SrcViewWindow == null || textureHeight <= SrcViewWindow.Size.H)
            {
                return 0;
            }
            return textureHeight - SrcViewWindow.Size.H;
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

                ScreenToWorld(new Point(screenX, screenY), out Point worldPos);

                Cell? cell = World.Map.GetCell(worldPos);

                if (cell != null)
                {
                    World.Map.SelectedCell = cell;

                    SelectRect = new SDL.SDL_Rect
                    {
                        x = cell.Value.WorldPos.X,
                        y = cell.Value.WorldPos.Y,
                        w = World.Map.Grid.CellSize.W,
                        h = World.Map.Grid.CellSize.H,
                    };
                }
            }
        }
    }
}
