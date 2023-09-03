using SDL2;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WordAlchemy.WorldGen;

namespace WordAlchemy
{
    public static class FontName
    {
        public static readonly string IBM_VGA_8X14 = "Px437_IBM_VGA_8x14";
        public static readonly string UNIFONT = "unifont";
        public static readonly string COURIER_PRIME = "CourierPrime";
        public static readonly string FREEMONO = "FreeMono";
    }

    internal class GlyphAtlas
    {
        public Dictionary<string, Font> Fonts { get; set; }

        public Dictionary<IntPtr, Dictionary<char, SDL.SDL_Rect>> Glyphs { get; set; }

        public Dictionary<IntPtr, IntPtr> FontTextures { get; set; }

        public readonly int TEXTURE_SIZE = 512;

        public readonly string ASCII = " \n1234567890-=!@#$%^&*()_+qwertyuiop[]\\asdfghjkl;'zxcvbnm,./QWERTYUIOP{}|ASDFGHJKL:ZXCVBNM<>?`~\"";
        public readonly string[] ExtraChars = new string[]
        {
            Terrain.Hill.Symbol, 
            Terrain.Mountain.Symbol, 
        };

        private SDLGraphics Graphics { get; set; }  

        public GlyphAtlas()
        {
            Fonts = new Dictionary<string, Font>();
            Glyphs = new Dictionary<IntPtr, Dictionary<char, SDL.SDL_Rect>>();
            FontTextures = new Dictionary<IntPtr, IntPtr>();

            Graphics = SDLGraphics.Instance;
        }

        public bool AddFont(Font font)
        {
            Fonts.Add(font.Name, font);
            Glyphs.Add(font.TTFFont, new Dictionary<char, SDL.SDL_Rect>());

            IntPtr inBetweenSurface, finalSurface;
            SDL.SDL_Rect dest = new SDL.SDL_Rect();

            finalSurface = SDL.SDL_CreateRGBSurface(0, TEXTURE_SIZE, TEXTURE_SIZE, 32, 0, 0, 0, 0xff);

            SDL.SDL_Surface surface = new SDL.SDL_Surface();
            
            if (finalSurface != IntPtr.Zero)
            {
                object? surfaceObj = Marshal.PtrToStructure(finalSurface, typeof(SDL.SDL_Surface));
                if (surfaceObj != null)
                {
                    surface = (SDL.SDL_Surface)surfaceObj;
                }
                
            }
            
            SDL.SDL_SetColorKey(finalSurface, 1, SDL.SDL_MapRGBA(surface.format, 0, 0, 0, 0));

            string allChars = ASCII + ExtraChars.Aggregate((s1, s2) => s1 + s2); 

            foreach (char c in  allChars)
            {
                string cStr = $"{c}";

                inBetweenSurface = SDL_ttf.TTF_RenderUTF8_Blended(font.TTFFont, cStr, Colors.White());

                if (inBetweenSurface != IntPtr.Zero)
                {
                    SDL_ttf.TTF_SizeText(font.TTFFont, cStr, out dest.w, out dest.h);

                    if (dest.x + dest.w > TEXTURE_SIZE)
                    {
                        dest.x = 0;
                        dest.y += dest.h + 1;

                        if (dest.y + dest.h > TEXTURE_SIZE)
                        {
                            Debug.WriteLine($"Out of glyph space for font {font.Name}");
                            return false;
                        }
                    }

                    SDL.SDL_BlitSurface(inBetweenSurface, IntPtr.Zero, finalSurface, ref dest);

                    SDL.SDL_Rect glyph = new SDL.SDL_Rect
                    {
                        x = dest.x,
                        y = dest.y,
                        w = dest.w,
                        h = dest.h,
                    };

                    Glyphs[font.TTFFont].Add(c, glyph);

                    SDL.SDL_FreeSurface(inBetweenSurface);

                    dest.x += dest.w;
                }
            }

            FontTextures.Add(font.TTFFont, Graphics.CreateTextureFromSurface(finalSurface));

            return true;
        }

        public void Draw()
        {
            Font font = Fonts[FontName.IBM_VGA_8X14];

            IntPtr texture = FontTextures[font.TTFFont];

            SDL.SDL_QueryTexture(texture, out _, out _, out int w, out int h);

            Graphics.DrawTexture(FontTextures[font.TTFFont], 460 - w / 2, 340 - h / 4);
        }
    }
}
