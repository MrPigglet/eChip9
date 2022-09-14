using SDL2;
using System;
using System.IO;
using System.Threading;

namespace eChip9
{
    public class eChip9
    {
        public SDL.SDL_Event input;
        public bool powerSwitch;
        public CPU CPU;
        public Input keyboard;
        public eChip9()
        {
            CPU = new CPU();
            powerSwitch = true; // Bool that will turn off when the emulator is supposed to turn off
            keyboard = new Input();
            LoadRom("DefaultMemory.ch8", 0); // Load default Fonts into memory (bundled with application)
        }
        public void StartEmulation()
        {
            int speedup = 1;
            CPU.PC = 512; // Skip to Program
            while (powerSwitch && input.type != SDL.SDL_EventType.SDL_QUIT) // Quit out of the game-loop if the X is clicked or if the powerswitch is flipped
            {
                SDL.SDL_PollEvent(out input);
                switch (input.type)
                {
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        if (input.key.keysym.sym == SDL.SDL_Keycode.SDLK_RSHIFT) // If holding Right-Shift, make the program go FAST
                        {
                            speedup = 0;
                        }
                        else
                        {
                            keyboard.InputHandler(input, 0); // Send whichever key is pressed down to my input handler
                        }
                        break;
                    case SDL.SDL_EventType.SDL_KEYUP:
                        if (input.key.keysym.sym == SDL.SDL_Keycode.SDLK_RSHIFT) // If releasing Right-Shitft, return to normal program speed
                        {
                            speedup = 1;
                        }
                        else
                        {
                            keyboard.InputHandler(input, 1); // Send whichever key is released to my input handler
                        }
                        break;
                    default: 
                        break;
                }
                ushort opcode = (ushort)((CPU.RAM[CPU.PC] << 8) | CPU.RAM[CPU.PC + 1]); // Combine two bytes in memory into an opcode
                try
                {
                    // PowerSwitch will be set to false in the rare case that an opcode requests it
                    powerSwitch = CPU.ExecuteOpcode(opcode, keyboard); // Send the opcode to the CPU!
                    Thread.Sleep(speedup); // Sleep to not make the emulation too fast
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); // Mostly for testing, should never trigger in real use
                }
            }
            Window.Close(); // Close the entire application
        }
        public void LoadRom(string filePath, int startPoint) // Load ROM into RAM at specified memory location
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    CPU.RAM[startPoint + reader.BaseStream.Position] = reader.ReadByte(); // Copy info from file, byte per byte, into virtual RAM
                }
            }
        }
    }
}
