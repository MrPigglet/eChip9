using SDL2;
using System;

namespace eChip9
{
    public class AudioDevice // I never got this to work :(
    {
        public SDL.SDL_AudioSpec audioSpec;
        public AudioDevice()
        {
            SDL.SDL_Init(SDL.SDL_INIT_AUDIO);
            audioSpec.freq = 44100;
            audioSpec.format = SDL.AUDIO_S16SYS;
            audioSpec.channels = 1;
            audioSpec.samples = 4096;
            //audioSpec.callback = SDL.SDL_AudioCallback;
            audioSpec.userdata = IntPtr.Zero;
        }
    }
}
