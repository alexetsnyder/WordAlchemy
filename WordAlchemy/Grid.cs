using SDL2;
using System.Diagnostics;

namespace WordAlchemy
{
    internal class Grid
    {
        public bool IsVisible { get; set; }

        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        public int ScreenOriginX { get; private set; }
        public int ScreenOriginY { get; private set; }

        public int WorldOriginX { get; set; }
        public int WorldOriginY { get; set; }

        public int OriginOffsetX { get; set; }
        public int OriginOffsetY { get; set; }

        public int CellWidth { get; set; }

        public bool IsCellSelected { get; set; }
        public int CellSelectedX { get; set; }
        public int CellSelectedY { get; set; }

        private SDLGraphics Graphics { get; set; }

        public Grid(int windowWidth, int windowHeight)
        {
            IsVisible = false;

            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            WorldOriginX = 0;
            WorldOriginY = 0;

            ScreenOriginX = windowWidth / 2;
            ScreenOriginY = windowHeight / 2;

            OriginOffsetX = 0;
            OriginOffsetY = 0;

            CellWidth = 20;

            IsCellSelected = false;
            CellSelectedX = 0;
            CellSelectedY = 0;

            Graphics = SDLGraphics.Instance;

            WireEvents();
        }

        public void WireEvents()
        {
            EventSystem eventSystem = EventSystem.Instance;
            eventSystem.Listen(SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN, MouseButtonDownEvent);
        }

        public void Draw()
        {
            if (IsCellSelected )
            {
                Graphics.SetDrawColor(Colors.Red());

                CellToScreen(CellSelectedX, CellSelectedY, out int screenX, out int  screenY);

                SDL.SDL_Rect rect = new SDL.SDL_Rect()
                {
                    x = screenX,
                    y = screenY,
                    w = CellWidth,
                    h = CellWidth,
                };

                Graphics.FillRect(ref rect);
            }

            if (IsVisible)
            {
                Graphics.SetDrawColor(Colors.SlateGrey());

                int modOffsetX = OriginOffsetX % CellWidth;
                int modOffsetY = OriginOffsetY % CellWidth;

                int gridStartX = ScreenOriginX + ((OriginOffsetX < 0) ? modOffsetX + CellWidth : modOffsetX);
                int gridStartY = ScreenOriginY + ((OriginOffsetY < 0) ? modOffsetY + CellWidth : modOffsetY);

                for (int x = gridStartX; x >= 0; x -= CellWidth)
                {
                    Graphics.DrawLine(x, 0, x, WindowHeight);
                }

                for (int x = gridStartX + CellWidth; x <= WindowWidth; x += CellWidth)
                {
                    Graphics.DrawLine(x, 0, x, WindowHeight);
                }

                for (int y = gridStartY; y >= 0; y -= CellWidth)
                {
                    Graphics.DrawLine(0, y, WindowWidth, y);
                }

                for (int y = gridStartY + CellWidth; y <= WindowHeight; y += CellWidth)
                {
                    Graphics.DrawLine(0, y, WindowWidth, y);
                }
            } 
        }

        public void SetWindowSize(int width, int height)
        {
            WindowWidth = width;
            WindowHeight = height;

            ScreenOriginX = WindowWidth / 2;
            ScreenOriginY = WindowHeight / 2;
        }

        public void SelectCell(int screenX, int screenY)
        {
            ScreenToCell(screenX, screenY, out int cellX, out int cellY);

            if (IsCellSelected && CellSelectedX == cellX && CellSelectedY == cellY)
            {
                IsCellSelected = false;
            }
            else
            {
                IsCellSelected = true;
                CellSelectedX = cellX;
                CellSelectedY = cellY;
            }
        }

        public bool IsOnScreen(int cellX,  int cellY)
        {
            CellToScreen(cellX, cellY, out int screenX, out int screenY);
            if (screenX > -CellWidth && screenY <= WindowWidth &&
                screenY > -CellWidth && screenY <= WindowHeight)
            {
                return true;
            }
            return false;
        }

        public void ScreenToCell(int screenX, int screenY, out  int cellX, out int cellY)
        {
            int tempX = screenX + (WorldOriginX - ScreenOriginX - OriginOffsetX);
            int tempY = screenY + (WorldOriginY - ScreenOriginY - OriginOffsetY);

            int modTempX = (tempX < 0) ? (tempX % CellWidth) + CellWidth : tempX % CellWidth;
            int modTempY = (tempY < 0) ? (tempY % CellWidth) + CellWidth : tempY % CellWidth;

            cellX = tempX - modTempX;
            cellY = tempY - modTempY;
        }

        public void CellToScreen(int cellX, int cellY, out int screenX, out int screenY)
        {
            screenX = cellX - (WorldOriginX - ScreenOriginX - OriginOffsetX);
            screenY = cellY - (WorldOriginY - ScreenOriginY - OriginOffsetY);
        }

        private void MouseButtonDownEvent(SDL.SDL_Event e)
        {
            if (e.button.button == SDL.SDL_BUTTON_LEFT)
            {
                SDL.SDL_GetMouseState(out int x, out int y);
                Debug.WriteLine($"Mouse X: {x}, Mouse Y: {y}");

                ScreenToCell(x, y, out int cellX, out int cellY);
                Debug.WriteLine($"Cell X: {cellX}, Cell Y: {cellY}");

                CellToScreen(cellX, cellY, out int screenX, out int screenY);
                Debug.WriteLine($"Screen X: {screenX}, Screen Y: {screenY}");

                SelectCell(x, y);
            }
        }
    }
}
