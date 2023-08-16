﻿using SDL2;

namespace WordAlchemy
{
    internal class SDLGraphics
    {
        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        public SDL.SDL_Color ClearColor { get; set; }

        IntPtr Window { get; set; }
        IntPtr Renderer { get; set; }

        public SDLGraphics(int width = 1040, int height = 880) 
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
                Console.WriteLine($"There was an issue initializing SDL. {SDL.SDL_GetError()}");
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
                Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
                return false;
            }

            Renderer = SDL.SDL_CreateRenderer(
                Window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

            if (Renderer == IntPtr.Zero)
            {
                Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
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

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            SDL.SDL_RenderDrawLine(Renderer, x1, y1, x2, y2);
        }
    }
}
