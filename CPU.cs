using System;
using System.Collections.Generic;

namespace eChip9
{
    public class CPU
    {
        // Components / Variables
        public byte[] RAM = new byte[4096]; 
        public byte[] V = new byte[16]; // 16 8-bit general purpouse registers
        public ushort I = 0; // One 16-bit register
        public DecreaseTimer delayTimer = new DecreaseTimer(); // Two timers, both decreasing at a rate of 60 hz
        public DecreaseTimer soundTimer = new DecreaseTimer();
        public Stack<ushort> Stack = new Stack<ushort>(16); // Stack for subroutines (technically infinite, even though the CHIP-8 will only ever use 16)
        public ushort PC; // Program Counter (Where in RAM it should read the next opcode)
        public byte[,] Display = new byte[64, 32]; // 64x32 display
        public bool ExecuteOpcode(ushort opcode, Input keyInput)
        {
            byte fFourBits = (byte)((opcode & 0xF000) >> 8); // We are going to mask out the first four Bits to find which opcode is wanted
            int x = (opcode & 0x0F00) >> 8; // Set x-variable (some opcodes will use this)
            int y = (opcode & 0x00F0) >> 4; // Set y-variable (some opcodes will use this)
            switch (fFourBits)
            {
                case 0x00: // There are multiple opcodes starting with 0
                    if (opcode == 0x00E0) // Clear the screen
                    {
                        Display = new byte[64, 32];
                        Window.ClearScreen();
                    }
                    else if (opcode == 0x00EE) // Return from subroutine
                    {
                        PC = Stack.Peek();
                        Stack.Pop();
                    }
                    else throw new Exception($"Opcode not supported {opcode.ToString("X4")}");
                    break;
                case 0x10: // Jump to location in nnn in memory (nnn from opcode)
                    PC = (ushort)(opcode & 0x0FFF);
                    return true; 
                case 0x20: // Call subroutine at nnn in memory (nnn from opcode)
                    Stack.Push(PC);
                    PC = (ushort)(opcode & 0x0FFF);
                    return true; 
                case 0x30: // Skip the next instruction if register V[x] == kk (kk from opcode)
                    if (V[x] == (byte)(opcode & 0x00FF))
                    {
                        PC += 2;
                    }
                    break;
                case 0x40: // Skip the next instruction if register V[x] != kk (kk from opcode)
                    if (V[x] != (byte)(opcode & 0x00FF))
                    {
                        PC += 2;
                    }
                    break;
                case 0x50: // Skip the next instruction if V[x] == V[y]
                    if (V[x] == V[y])
                    {
                        PC += 2;
                    }
                    break;
                case 0x60: // Set V[x] to kk (kk from opcode)
                    V[x] = (byte)(opcode & 0x00FF);
                    break;
                case 0x70: // Adds kk to V[x] (kk from opcode) and stores the result in V[x]
                    V[x] = (byte)(V[x] + (opcode & 0x00FF));
                    break;
                case 0x80: // There are multiple opcodes starting with 8
                    switch (opcode & 0x000F)
                    {
                        case 0x0000: // Store the value of V[y] in V[x]
                            V[x] = V[y];
                            break;
                        case 0x0001: // Preform a bitwise OR on V[x] and V[y] and store the result in V[x]
                            V[x] = (byte)(V[x] | V[y]);
                            break;
                        case 0x0002: // Preform a bitwise AND on V[x] and V[y] and store the result in V[x]
                            V[x] = (byte)(V[x] & V[y]);
                            break;
                        case 0x0003: // Preform a bitwise exclusive OR on V[x] and V[y] and store the result in V[x]
                            V[x] = (byte)(V[x] ^ V[y]);
                            break;
                        case 0x0004: // Add V[x] and V[y] together and store the result in V[x]. If the result is too large to store in V[x], set V[15] to 1, otherwise set it to 0.
                            V[15] = (byte)(((V[x] + V[y]) > 255) ? 1 : 0);
                            V[x] = (byte)(V[x] + V[y]);
                            break;
                        case 0x0005: // Subtract V[y] from V[x] and store the result in V[x]. If V[x] is larger than V[y], set V[15] to 1, otherwise set it to 0.
                            V[15] = (byte)((V[x] > V[y]) ? 1 : 0);
                            V[x] = (byte)(V[x] - V[y]);
                            break;
                        case 0x0006: // If the bit furthest to the right in V[x] is 1, set V[15] to 1. Then Divide V[x] by 2 and store the result in V[x]
                            V[15] = (byte)(((V[x] & 0x01) == 0x01) ? 1 : 0);
                            V[x] = (byte)(V[x] >> 1);
                            break;
                        case 0x0007: // Subtract V[x] from V[y] and store the result in V[y]. If V[y] is larger than V[x], set V[5] to 1
                            V[15] = (byte)(V[y] > V[x] ? 1 : 0);
                            V[x] = (byte)(V[y] - V[x]);
                            break;
                        case 0x000E: // If the bit furthest to the left in V[x] is 1, set V[15] to 1. Multiply V[x] by two and store the result in V[x]
                            V[15] = (byte)(((V[x] & 0x80) == 0x80) ? 1 : 0);
                            V[x] = (byte)(V[x] << 1);
                            break;
                        default:
                            throw new Exception($"Opcode not supported {opcode.ToString("X4")}");
                    }
                    break;
                case 0x90: // Skip the next instruction if V[x] != V[y]
                    if (V[x] != V[y])
                    {
                        PC += 2;
                    }
                    break;
                case 0xA0: // Set I to nnn (nn from opcode)
                    I = (ushort)(opcode & 0x0FFF);
                    break;
                case 0xB0: // Set PC to nnn + V[0] (nnn from opcode)
                    PC = (ushort)((opcode & 0x0FFF) + V[0]);
                    return true;
                case 0xC0: // Preform a bitwise AND on a random Byte and kk (kk from opcode) and store the result in V[x]
                    V[x] = (byte)(ThreadSafeRandom.ThisThreadsRandom.Next(0, 256) & (opcode & 0x00FF));
                    break;
                case 0xD0: // Display sprite from memory stored at RAM[I]. If a pixel is removed, set V[15] to 1, otherwise set V[15] to 0.
                    V[15] = 0;
                    int spriteHeight = opcode & 0x000F;
                    if (spriteHeight == 0) { spriteHeight = 16; }
                    for (int i = 0; i < spriteHeight; i++) // To cycle through the rows of the sprite
                    {
                        for (int j = 0; j < 8; j++) // To cycle through each Bit in every Byte of sprite memory
                        {
                            if ((byte)((RAM[I + i] >> (7 - j)) & 0x01)  == 0x01) // Check if the bit is 1 ( if anything has to change )
                            {
                                // At this point we need to validate V[x] + j and V[y] + i
                                int _x = V[x] + j;
                                int _y = V[y] + i;
                                while (_x > (Display.GetLength(0) - 1)) { _x -= 64; }
                                while (_x < 0) { _x += 64; }
                                while (_y > (Display.GetLength(1) - 1)) { _y -= 32; }
                                while (_y < 0) { _y += 32; }
                                // Validation done, we draw the pixelsinput


                                if (Display[_x, _y] == 0x01) // XOR - Remove Pixel
                                {
                                    Display[_x, _y] = 0x00;
                                    Window.DrawPixel(_x, _y, 1);
                                    V[15] = 0x01;
                                }
                                else // XOR - Add Pixel
                                {
                                    Display[_x, _y] = 0x01;
                                    Window.DrawPixel(_x, _y, 0);
                                }
                            }
                        }
                    }
                    Window.UpdateDisplay(); // Only update the display once the entire sprite is ready to be printed
                    break;
                case 0xE0: // There are multiple opcodes starting with E
                    if ((opcode & 0xF0FF) == 0xE09E) // Skip the next instruction if the key with the value V[x] is pressed
                    {
                        if (keyInput.CheckActiveKeys(V[x]))
                        {
                            PC += 2;
                        }
                    }
                    else if ((opcode & 0xF0FF) == 0xE0A1) // Skip the next instruction if the key with the value of V[x] is not pressed
                    {
                        if (!keyInput.CheckActiveKeys(V[x]))
                        {
                            PC += 2;
                        }
                    }
                    break;
                case 0xF0: // There are multiple opcodes starting with F
                    switch (opcode & 0x00FF)
                    {
                        case 0x0007: // Store the value of the delay timer into V[x]
                            V[x] = delayTimer.value;
                            break;
                        case 0x000A: // Pause everything until a valid keypress is recieved, store that keypress in V[x]
                            V[x] = keyInput.WaitForInput();
                            if (V[x] == 0xFF)
                            {
                                return false; // Turn the emulator off if X is pressed
                            }
                            break;
                        case 0x0015: // Set the delay timer to the value of V[x]
                            delayTimer.value = V[x];
                            break;
                        case 0x0018: // Set the sound timer to the value of V[x]
                            soundTimer.value = V[x];
                            break;
                        case 0x001E: // Add V[x] to I and store the result in I
                            I += V[x];
                            break;
                        case 0x0029: // Set I to the location in memory corresponding to the default font number equal to V[x]
                            I = (ushort)(V[x] * 5);
                            break;
                        case 0x0033: // Store the digits of V[x] in RAM[I], RAM[I + 1] and RAM[I + 2]
                            int storage = V[x] % 10;
                            RAM[I + 2] = (byte)storage;
                            RAM[I + 1] = (byte)(((V[x] % 100) - storage) / 10);
                            storage = (V[x] % 100) - storage;
                            RAM[I] = (byte)(((V[x] % 1000) - storage) / 100);
                            break;
                        case 0x0055: // Store all registers from V[0] to V[x] into RAM, starting at memory location I
                            for (int i = 0; i <= x; i++)
                            {
                                RAM[I + i] = V[i];
                            }
                            break;
                        case 0x0065: // Store the values of RAM, starting at location I, into registers V[0] through V[x]
                            for (int i = 0; i <= x; i++)
                            {
                                V[i] = RAM[I + i];
                            }
                            break;
                    }
                    break;
                default:
                    throw new Exception($"Opcode not supported {opcode.ToString("X4")}");
            }

            // Sometimes we return in this switch-statement instead of breaking 
            // to not increment the PC by 2

            PC += 2; // Increment the PC by 2
            return  true;
        }
    }
}
