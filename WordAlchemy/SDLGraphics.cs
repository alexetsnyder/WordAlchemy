using SDL2;
using System.Diagnostics;

namespace WordAlchemy
{
    internal class SDLGraphics
    {
        public static SDLGraphics Instance { get { return Nested.instance; } }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public GlyphAtlas? Atlas { get; set; }

        public SDL.SDL_Color ClearColor { get; set; }

        private IntPtr Window { get; set; }
        private IntPtr Renderer { get; set; }

        private SDLGraphics() 
        {
            WindowWidth = 0;
            WindowHeight = 0;

            Atlas = null;
            SetClearColor(Colors.Black());

            Window = IntPtr.Zero;
            Renderer = IntPtr.Zero;
        }

        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly SDLGraphics instance = new SDLGraphics();
        }

        public bool Init(int windowWidth, int windowHeight)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;

            Atlas = new GlyphAtlas();

            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
            {
                Debug.WriteLine($"There was an issue initializing SDL. {SDL.SDL_GetError()}");
                return false;
            }

            Window = SDL.SDL_CreateWindow("Word Alchemy",
                SDL.SDL_WINDOWPOS_UNDEFINED,
                SDL.SDL_WINDOWPOS_UNDEFINED,
                WindowWidth,
                WindowHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN |
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            if (Window == IntPtr.Zero)
            {
                Debug.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
                return false;
            }

            Renderer = SDL.SDL_CreateRenderer(
                Window,
                -1,
                SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED |
                SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC |
                SDL.SDL_RendererFlags.SDL_RENDERER_TARGETTEXTURE);

            if (Renderer == IntPtr.Zero)
            {
                Debug.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
                return false;
            }

            if (SDL_ttf.TTF_Init() < 0)
            {
                Debug.WriteLine($"Couldn't initialize SDL TTF: {SDL.SDL_GetError()}");
                return false;
            }

            Atlas.AddFont(new Font(FontName.UNIFONT, "Assets/Fonts/unifont.ttf", 18));
            Atlas.AddFont(new Font(FontName.COURIER_PRIME, "Assets/Fonts/Courier Prime.ttf", 18));
            Atlas.AddFont(new Font(FontName.FREEMONO, "Assets/Fonts/FreeMono.ttf", 18));

            return true;
        }

        public void SetClearColor(SDL.SDL_Color clearColor)
        {
            SetClearColor(clearColor.r, clearColor.g, clearColor.b, clearColor.a);
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

        public void SetDrawColor(SDL.SDL_Color color)
        {
            SetDrawColor(color.r, color.g, color.b, color.a);
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
            SDL_ttf.TTF_Quit();
            SDL.SDL_DestroyRenderer(Renderer);
            SDL.SDL_DestroyWindow(Window);
            SDL.SDL_Quit();
        }

        public void SizeText(string text, string fontName, out int width, out int height)
        {
            if (Atlas != null)
            {
                IntPtr fontPtr = Atlas.Fonts[fontName].TTFFont;
                SDL_ttf.TTF_SizeText(fontPtr, text, out width, out height);
            }
            else
            {
                width = 0;
                height = 0;
            }
        }

        public IntPtr CreateTextureFromSurface(IntPtr surface)
        {
            return SDL.SDL_CreateTextureFromSurface(Renderer, surface);
        }

        public IntPtr CreateTexture(int width, int height)
        {
            return SDL.SDL_CreateTexture(Renderer, SDL.SDL_PIXELFORMAT_RGBA8888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, width, height);
        }

        public void DrawPoint(int x, int y)
        {
            SDL.SDL_RenderDrawPoint(Renderer, x, y);
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

        public void DrawTextToTexture(IntPtr dstTexture, string text, int x, int y, SDL.SDL_Color color, string fontName)
        {
            SDL.SDL_SetTextureBlendMode(dstTexture, SDL.SDL_BlendMode.SDL_BLENDMODE_BLEND);
            SDL.SDL_SetRenderTarget(Renderer, dstTexture);

            DrawText(text, x, y, color, fontName);

            SDL.SDL_SetRenderTarget(Renderer, IntPtr.Zero);
        }

        public void DrawText(string text, int x, int y, SDL.SDL_Color color, string fontName)
        {
            if (Atlas != null)
            {
                IntPtr font = Atlas.Fonts[fontName].TTFFont;

                SDL.SDL_SetTextureColorMod(Atlas.FontTextures[font], color.r, color.g, color.b);

                foreach (char c in text)
                {
                    SDL.SDL_Rect glyph = Atlas.Glyphs[font][c];

                    SDL.SDL_Rect dest;
                    dest.x = x;
                    dest.y = y;
                    dest.w = glyph.w;
                    dest.h = glyph.h;

                    SDL.SDL_RenderCopy(Renderer, Atlas.FontTextures[font], ref glyph, ref dest);

                    x += glyph.w;
                }
            }    
        }

        public void DrawTexture(IntPtr texture, int x, int y)
        {
            SDL.SDL_Rect dest = new SDL.SDL_Rect
            {
                x = x,
                y = y,
            };

            SDL.SDL_QueryTexture(texture, out _, out _, out dest.w, out dest.h);

            DrawTexture(texture, ref dest);
        }

        public void DrawTexture(IntPtr texture, ref SDL.SDL_Rect dest)
        {
            SDL.SDL_RenderCopy(Renderer, texture, IntPtr.Zero, ref dest);
        }
    }
}
