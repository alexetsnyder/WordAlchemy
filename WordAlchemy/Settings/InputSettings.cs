using SDL2;

namespace WordAlchemy.Settings
{
    public class InputSettings
    {
        public static InputSettings Instance { get { return Nested.instance; } }

        public SDL.SDL_Keycode MapUp { get; set; }
        public SDL.SDL_Keycode MapDown { get; set; }
        public SDL.SDL_Keycode MapLeft { get; set; }
        public SDL.SDL_Keycode MapRight { get; set; }

        public SDL.SDL_Keycode PlayerUp { get; set; }
        public SDL.SDL_Keycode PlayerDown { get; set; }
        public SDL.SDL_Keycode PlayerLeft { get; set; }
        public SDL.SDL_Keycode PlayerRight { get; set; }

        public SDL.SDL_Keycode MapButton { get; set; }

        public uint MouseButtonSelect { get; set; }

        private InputSettings()
        {
            MapUp = SDL.SDL_Keycode.SDLK_w;
            MapDown = SDL.SDL_Keycode.SDLK_s;
            MapLeft = SDL.SDL_Keycode.SDLK_a;
            MapRight = SDL.SDL_Keycode.SDLK_d;

            PlayerUp = SDL.SDL_Keycode.SDLK_w;
            PlayerDown = SDL.SDL_Keycode.SDLK_s;
            PlayerLeft = SDL.SDL_Keycode.SDLK_a;
            PlayerRight = SDL.SDL_Keycode.SDLK_d;

            MapButton = SDL.SDL_Keycode.SDLK_m;

            MouseButtonSelect = SDL.SDL_BUTTON_LEFT;
        }

        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly InputSettings instance = new InputSettings();
        }

    }
}
