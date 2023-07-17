using SDL2;
using System;

IntPtr window = IntPtr.Zero;
IntPtr renderer = IntPtr.Zero;
bool isRunning = true;

int WINDOW_WIDTH = 1040;
int WINDOW_HEIGHT = 880;

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
        WINDOW_WIDTH, 
        WINDOW_HEIGHT, 
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

    DrawGrid();

    SDL.SDL_RenderPresent(renderer);
}

void DrawGrid()
{
    int width = 10;

    SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 255);

    for (int x = 0; x < WINDOW_WIDTH; x += width)
    {
        SDL.SDL_RenderDrawLine(renderer, x, 0, x, WINDOW_HEIGHT);
    }

    for (int y = 0; y < WINDOW_HEIGHT; y += width)
    {
        SDL.SDL_RenderDrawLine(renderer, 0, y, WINDOW_WIDTH, y);
    }
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
