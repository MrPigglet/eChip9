using SDL2;

namespace eChip9
{
    public class Input
    {
        // An array of bools corresponding to each of the 16 keys a CHIP-8 can handle. This is how the program can know what is being held down or not
        public bool[] activeKeys = new bool[17] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false};
        public void InputHandler(SDL.SDL_Event keyEvent, int state)
        {
            switch (state)
            {
                case 0: // If pressed down, set the corresponding activeKey to true
                    activeKeys[InputTranslate(keyEvent)] = true;
                    break;
                case 1: // If released, set the corresponding activeKey to false
                    activeKeys[InputTranslate(keyEvent)] = false;
                    break;
                default:
                    break;
            }
        }
        public byte InputTranslate(SDL.SDL_Event translate) // Translate keys form a modern keyboard to a CHIP-8 keyboard
        {
            switch (translate.key.keysym.sym)
            {
                case SDL.SDL_Keycode.SDLK_1:
                    return 0x01;
                case SDL.SDL_Keycode.SDLK_2:
                    return 0x02;
                case SDL.SDL_Keycode.SDLK_3:
                    return 0x03;
                case SDL.SDL_Keycode.SDLK_4:
                    return 0x0C;
                case SDL.SDL_Keycode.SDLK_q:
                    return 0x04;
                case SDL.SDL_Keycode.SDLK_w:
                    return 0x05;
                case SDL.SDL_Keycode.SDLK_e:
                    return 0x06;
                case SDL.SDL_Keycode.SDLK_r:
                    return 0x0D;
                case SDL.SDL_Keycode.SDLK_a:
                    return 0x07;
                case SDL.SDL_Keycode.SDLK_s:
                    return 0x08;
                case SDL.SDL_Keycode.SDLK_d:
                    return 0x09;
                case SDL.SDL_Keycode.SDLK_f:
                    return 0x0E;
                case SDL.SDL_Keycode.SDLK_z:
                    return 0x0A;
                case SDL.SDL_Keycode.SDLK_x:
                    return 0x00;
                case SDL.SDL_Keycode.SDLK_c:
                    return 0x0B;
                case SDL.SDL_Keycode.SDLK_v:
                    return 0x0F;
                default:
                    return 0x10; // Return key that is not allowed, won't affect game at all
            }
        }
        public bool CheckActiveKeys(byte compare)
        {
            return activeKeys[compare]; // Check if specific key is true (held down)
        }
        public byte WaitForInput() // Pause the emulation and wait for input
        {
            SDL.SDL_Event keyEvent;
            while ((SDL.SDL_PollEvent(out keyEvent) >= 0) && true)
            {
                switch (keyEvent.type)
                {
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        if (InputTranslate(keyEvent) != 17)
                        {
                            InputHandler(keyEvent, 0);
                            return InputTranslate(keyEvent);
                        }
                        break;
                    case SDL.SDL_EventType.SDL_QUIT: // If X is pressed, quit application. This will flip the powerswitch
                        return 0xFF;
                    default:
                        break;
                }
            }
            return default;
        }
    }
}
