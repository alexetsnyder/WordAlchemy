using SDL2;

namespace WordAlchemy
{
    internal class Grid
    {
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

        public Grid(int windowWidth, int windowHeight)
        {
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
        }

        public void Draw(SDLGraphics graphics)
        {
            graphics.SetDrawColor(0, 0, 0, 255);

            if (IsCellSelected )
            {
                graphics.SetDrawColor(255, 0, 0, 255);

                CellToScreen(CellSelectedX, CellSelectedY, out int screenX, out int  screenY);

                SDL.SDL_Rect rect = new SDL.SDL_Rect()
                {
                    x = screenX,
                    y = screenY,
                    w = CellWidth,
                    h = CellWidth,
                };

                graphics.FillRect(ref rect);
            }

            graphics.SetDrawColor(0, 0, 0, 255);

            for (int x = ScreenOriginX; x >= 0; x -= CellWidth)
            {
                graphics.DrawLine(x, 0, x, WindowHeight);
            }

            for (int x = ScreenOriginX + CellWidth; x <= WindowWidth; x += CellWidth)
            {
                graphics.DrawLine(x, 0, x, WindowHeight);
            }

            for (int y = ScreenOriginY; y >= 0; y -= CellWidth)
            {
                graphics.DrawLine(0, y, WindowWidth, y);
            }

            for (int y = ScreenOriginY + CellWidth; y <= WindowHeight; y += CellWidth)
            {
                graphics.DrawLine(0, y, WindowWidth, y);
            }
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
            if (screenX >= 0 && screenY <= WindowWidth &&
                screenY >= 0 && screenY <= WindowHeight)
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
    }
}
