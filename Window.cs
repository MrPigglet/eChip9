using System;
using SDL2;

namespace eChip9
{
    class Window
    {
        private int height;
        private int width;
        private int scale = 10; // Scale of window compared to real size
        static IntPtr window;
        static IntPtr renderer;
        public Window(int height, int width)
        {
            this.height = height * scale;
            this.width = width * scale;
            Init();
        }
        public void Init()
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            window = SDL.SDL_CreateWindow("eChip9", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, width, height, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            SDL.SDL_RenderSetScale(renderer, scale, scale);
            ClearScreen();
        }
        public static void DrawPixel(int x, int y, int colour)
        {
            switch (colour)
            {
                case 0: // Draw White
                    SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);
                    break;
                case 1: // Draw Black
                    SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
                    break;
            }
            SDL.SDL_RenderDrawPoint(renderer, x, y); // Set the pixel at the desired x-y location to whichever colour
        }
        public static void ClearScreen()
        {
            SDL.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0); // Black
            SDL.SDL_RenderClear(renderer); // Fill screen with colour
            UpdateDisplay();
        }
        public static void UpdateDisplay()
        {
            SDL.SDL_RenderPresent(renderer);
        }
        public static void Close()
        {
            SDL.SDL_DestroyWindow(window); // Clear window from ram to exit the program in a nice way
            window = IntPtr.Zero;
            SDL.SDL_Quit();
        }
    }
}
