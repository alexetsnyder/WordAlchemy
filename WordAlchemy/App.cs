
using SDL2;

namespace WordAlchemy
{
    internal class App
    {
        public bool IsRunning { get; set; }

        private SDLGraphics Graphics { get; set; }

        private Grid GGrid { get; set; }

        public App()
        {
            IsRunning = true;
            Graphics = new SDLGraphics();
            
            if (!Graphics.Init())
            {
                IsRunning = false;
            }

            GGrid = new Grid(1040, 880);
        }

        public void Run()
        {
            while (IsRunning)
            {
                PollEvents();

                Render();
            }

            Graphics.CleanUp();
        }

        private void PollEvents()
        {
            foreach (var e in Graphics.PollEvents())
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        IsRunning = false;
                        break;
                }
            }
        }

        private void Render()
        {
            Graphics.Clear();

            GGrid.Draw(Graphics);

            Graphics.Present();
        }
    }
}
