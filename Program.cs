using System;

namespace eChip9
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args[0] != null) // Do not start program if nothing is dropped onto it
            {
                Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // Fix path so loading in default memory rom works
                Window window = new Window(32, 64);
                eChip9 Emulator = new eChip9();
                Emulator.LoadRom(args[0], 512); // Load whatever rom has been drag-n-dropped onto exe into the correct part of memory
                Emulator.StartEmulation(); // Start
            }
        }
    }
}
