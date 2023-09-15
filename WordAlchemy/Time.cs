
using SDL2;

namespace WordAlchemy
{
    public class Time
    {
        public static Time Instance { get { return Nested.instance; } }

        public ulong PreviousTime { get; private set; } 

        public ulong CurrentTime { get; private set; }

        public int DeltaTime { get; private set; }

        private Time()
        {
            PreviousTime = 0;
            CurrentTime = 0;
            DeltaTime = 0;
        }

        private class Nested
        {
            static Nested()
            {

            }

            internal static readonly Time instance = new Time();
        }

        public void Init()
        {
            PreviousTime = SDL.SDL_GetPerformanceCounter();
            CurrentTime = SDL.SDL_GetPerformanceCounter();
        }

        public void Update()
        {
            PreviousTime = CurrentTime;
            CurrentTime = SDL.SDL_GetPerformanceCounter();
            DeltaTime = (int)((CurrentTime - PreviousTime) * 1000 / SDL.SDL_GetPerformanceFrequency());
        }
    }
}

