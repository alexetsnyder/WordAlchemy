﻿using SDL2;

namespace WordAlchemy.TextRendering
{
    internal class Font
    {
        public string Name { get; set; }

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

        public Font(string name, string filePath, int fontSize)
        {
            Name = name;
            FontSize = fontSize;
            FilePath = filePath;
            TTFFont = SDL_ttf.TTF_OpenFont(filePath, fontSize);
        }
    }
}
