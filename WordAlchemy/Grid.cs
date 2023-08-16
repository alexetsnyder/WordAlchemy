
namespace WordAlchemy
{
    internal class Grid
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public Grid(int windowWidth, int windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
        }

        public void Draw(SDLGraphics graphics)
        {
            int width = 10;

            //SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);
            graphics.SetDrawColor(0, 0, 0, 255);

            for (int x = 0; x < WindowWidth; x += width)
            {
                //SDL.SDL_RenderDrawLine(renderer, x, 0, x, WindowHeight);
                graphics.DrawLine(x, 0, x, WindowHeight);
            }

            for (int y = 0; y < WindowHeight; y += width)
            {
                //SDL.SDL_RenderDrawLine(renderer, 0, y, WindowWidth, y);
                graphics.DrawLine(0, y, WindowWidth, y);
            }
        }
    }
}
