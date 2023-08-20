using SDL2;

namespace WordAlchemy
{
    public static class Colors
    {
        public static SDL.SDL_Color Black()
        {
            return new SDL.SDL_Color
            {
                r = 0,
                g = 0,
                b = 0,
                a = 255,
            };
        }

        public static SDL.SDL_Color White()
        {
            return new SDL.SDL_Color
            {
                r = 255,
                g = 255,
                b = 255,
                a = 255,
            };
        }

        public static SDL.SDL_Color Brown()
        {
            return new SDL.SDL_Color
            {
                r = 150,
                g = 75,
                b = 0,
                a = 255,
            };
        }

        public static SDL.SDL_Color Red()
        {
            return new SDL.SDL_Color
            {
                r = 250,
                g = 0,
                b = 0,
                a = 255,
            };
        }

        public static SDL.SDL_Color Green()
        {
            return new SDL.SDL_Color
            {
                r = 0,
                g = 255,
                b = 0,
                a = 255,
            };
        }

        public static SDL.SDL_Color Blue()
        {
            return new SDL.SDL_Color
            {
                r = 0,
                g = 0,
                b = 255,
                a = 255,
            };
        }

        public static SDL.SDL_Color Pink()
        {
            return new SDL.SDL_Color
            {
                r = 255,
                g = 192,
                b = 203,
                a = 255,
            };
        }

        public static SDL.SDL_Color HotPink()
        {
            return new SDL.SDL_Color
            {
                r = 255,
                g = 105,
                b = 180,
                a = 255,
            };
        }

        public static SDL.SDL_Color NeonPink()
        {
            return new SDL.SDL_Color
            {
                r = 255,
                g = 16,
                b = 240,
                a = 255,
            };
        }

        public static SDL.SDL_Color RoseGold()
        {
            return new SDL.SDL_Color
            {
                r = 224,
                g = 191,
                b = 184,
                a = 255,
            };
        }
    }
}
