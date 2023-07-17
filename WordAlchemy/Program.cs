using SDL2;
using System;

IntPtr window = IntPtr.Zero;
IntPtr renderer = IntPtr.Zero;
bool isRunning = true;

void SetUp()
{
    if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
    {
        Console.WriteLine($"There was an issue initializing SDL. {SDL.SDL_GetError()}");
        isRunning = false;
        return;
    }

    window = SDL.SDL_CreateWindow("Word Alchemy", 
        SDL.SDL_WINDOWPOS_UNDEFINED, 
        SDL.SDL_WINDOWPOS_UNDEFINED, 
        640, 
        480, 
        SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);

    if (window == IntPtr.Zero )
    {
        Console.WriteLine($"There was an issue creating the window. {SDL.SDL_GetError()}");
        isRunning= false;
        return;
    }

    renderer = SDL.SDL_CreateRenderer(window, 
        -1, 
        SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | 
        SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC);

    if (renderer == IntPtr.Zero) 
    {
        Console.WriteLine($"There was an issue creating the renderer. {SDL.SDL_GetError()}");
        isRunning = false;
        return;
    }
}

void PollEvents()
{
    while (SDL.SDL_PollEvent(out SDL.SDL_Event e) == 1)
    {
        switch (e.type)
        {
            case SDL.SDL_EventType.SDL_QUIT:
                isRunning = false;
                break;
        }
    }
}

void Render()
{
    SDL.SDL_SetRenderDrawColor(renderer, 135, 206, 235, 255);

    SDL.SDL_RenderClear(renderer);

    SDL.SDL_RenderPresent(renderer);
}

void CleanUp()
{
    SDL.SDL_DestroyRenderer(renderer);
    SDL.SDL_DestroyWindow(window);
    SDL.SDL_Quit();
}

SetUp();

while (isRunning)
{
    PollEvents();
    Render();
}

CleanUp();

Console.WriteLine("Hello, World!");
