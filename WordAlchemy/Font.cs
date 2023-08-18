using SDL2;

namespace WordAlchemy
{
    internal class Font
    {
        private int _size;
        public int FontSize 
        { 
            get
            {
                return _size;
            }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    TTFFont = SDL_ttf.TTF_OpenFont(FilePath, value);
                } 
            }
        }

        private string FilePath { get; set; }

        public IntPtr TTFFont { get; private set; }

        public Font(string filePath, int fontSize)
        {
            FontSize = fontSize;
            FilePath = filePath;
            TTFFont = SDL_ttf.TTF_OpenFont(filePath, fontSize);
        }
    }
}
