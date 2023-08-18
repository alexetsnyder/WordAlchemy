using SDL2;

namespace WordAlchemy
{
    internal class SDLGraphics
    {
        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        public SDL.SDL_Color ClearColor { get; set; }

        IntPtr Window { get; set; }
        IntPtr Renderer { get; set; }

        public SDLGraphics(int width, int height) 
        {
            WindowWidth = width;
            WindowHeight = height;

            SetClearColor(135, 206, 235, 255);

            Window = IntPtr.Zero;
            Renderer = IntPtr.Zero;
        }

        public bool Init()
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                System.Diagnostics.Debug.WriteLine($"There was an issue initializing SDL. {SDL.SDL_GetError()}");
                return false;
            }

            Window = SDL.SDL_CreateWindow("Word Alchemy",
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                WindowWidth,
                WindowHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

            if (Window == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
                return false;
            }

            Renderer = SDL.SDL_CreateRenderer(
                Window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (Renderer == IntPtr.Zero)
            {
                System.Diagnostics.Debug.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
                return false;
            }

            return true;
        }

        public void SetClearColor(byte r, byte g, byte b, byte a)
        {
            ClearColor = new SDL.SDL_Color
            {
                r = r,
                g = g,
                b = b,
                a = a
            };
        }

        public void SetDrawColor(byte r, byte g,byte b,byte a)
        {
            SDL.SDL_SetRenderDrawColor(Renderer, r, g, b, a);
        }

        public void Clear()
        {
            SDL.SDL_SetRenderDrawColor(Renderer, ClearColor.r, ClearColor.g, ClearColor.b, ClearColor.a);
            SDL.SDL_RenderClear(Renderer);
        }

        public void Present()
        {
            SDL.SDL_RenderPresent(Renderer);
        }

        public IEnumerable<SDL.SDL_Event> PollEvents()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
            {
                yield return e;
            }
        }

        public void CleanUp()
        {
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(Window);
            SDL.SDL_Quit();
        }

        public IntPtr CreateTextureFromSurface(IntPtr surface)
        {
            return SDL.SDL_CreateTextureFromSurface(Renderer, surface);
        }

        public void DrawLine(int x1, int y1, int x2, int y2) 
        {
            SDL.SDL_RenderDrawLine(Renderer, x1, y1, x2, y2);
        }

        public void DrawRect(ref SDL.SDL_Rect rect)
        {
            SDL.SDL_RenderDrawRect(Renderer, ref rect);
        }

        public void FillRect(ref SDL.SDL_Rect rect)
        {
            SDL.SDL_RenderFillRect(Renderer, ref rect);
        }

        public void DrawTexture(IntPtr texture, int x, int y)
        {
            SDL.SDL_Rect dest = new SDL.SDL_Rect
            {
                x = x,
                y = y,
            };

            SDL.SDL_QueryTexture(texture, out _, out _, out dest.w, out dest.h);

            SDL.SDL_RenderCopy(Renderer, texture, IntPtr.Zero, ref dest);
        }
    }
}
