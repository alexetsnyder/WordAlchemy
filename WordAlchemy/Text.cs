using SDL2;

namespace WordAlchemy
{
    internal class Text
    {
        public Font TextFont { get; private set; }
        public string TextStr { get; private set; }
        public SDL.SDL_Color TextColor { get; private set; }

        public IntPtr Texture { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private bool IsTextureCreated { get; set; }

        private SDLGraphics Graphics { get; set; }
        
        public Text(Font font, string textStr, SDL.SDL_Color color)
        {
            TextFont = font;
            TextStr = textStr;
            TextColor = color;
            Texture = IntPtr.Zero;
            IsTextureCreated = false;
            Graphics = SDLGraphics.Instance;
            CreateTexture();
        }

        public void SetFont(Font font)
        {
            TextFont = font;
            IsTextureCreated = false;
        }

        public void SetText(string text)
        {
            TextStr = text;
            IsTextureCreated = false;
        }

        public void SetColor(SDL.SDL_Color color)
        {
            TextColor = color;
            IsTextureCreated = false;
        }

        public void CreateTexture()
        {
            IntPtr surface = SDL_ttf.TTF_RenderUTF8_Blended(TextFont.TTFFont, TextStr, TextColor);

            Texture = Graphics.CreateTextureFromSurface(surface);

            SDL.SDL_QueryTexture(Texture, out _, out _, out int w, out int h);
            Width = w;
            Height = h;
            IsTextureCreated = true;

            SDL.SDL_FreeSurface(surface);
        }

        public void Draw(int x, int y)
        {
            if (IsTextureCreated)
            {
                Graphics.DrawText(this, x, y);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Texture was not created have you called CreateTexture?");
            }
        }   
    }
}
