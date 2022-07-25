using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

// System.Byte for 8 bits
// System.UInt16 for 16 bits
// and so on
using scalar = System.Byte;

namespace Brainfuck_Interpreter
{
    class Program
    {
        const uint memorySize = 30000;

        static scalar[] memory;
        static uint   memoryIndex;

        static string instructions;
        static int instructionIndex;

        static FileStream   file;
        static StreamReader reader;

        static void Main(string[] args)
        {
            Console.InputEncoding  = Encoding.ASCII;
            Console.OutputEncoding = Encoding.ASCII;

            #region File Open

            Console.WriteLine("Please enter path to the file");
            string filePath = Console.ReadLine();

            try
            {
                file = new FileStream(filePath.Trim(new char[]{ '"' }), FileMode.Open, FileAccess.Read);
            }
            catch
            {
                Console.WriteLine("Wrong path. Interpretation terminated.");
                Environment.Exit(1);
                return;
            }
            
            reader = new StreamReader(file);

            #endregion

            instructions = reader.ReadToEnd() + " ";

            reader.Close();
            file.Close();

            int checkIndices = instructions.Count(x => x == ']') - instructions.Count(x => x == '[');
            if (checkIndices != 0)
            {
                Console.WriteLine("The amount of received '[' does not match the amount of received ']', detected mismatch of {0} (']' - '[')", checkIndices);
                Environment.Exit(2);
            }

            #region Runtime

            memory = new scalar[memorySize];

            instructionIndex = 0;
            memoryIndex = 0;

            Stopwatch clock = new Stopwatch();
            clock.Start();

            for(instructionIndex = 0; instructionIndex < instructions.Length - 1; instructionIndex++)
            {
                //Thread.Sleep(1000);
                PreInterpret();
                Interpret();
            }

            clock.Stop();

            #endregion

            Console.WriteLine();
            Console.WriteLine($"Finished in {clock.ElapsedMilliseconds} ms.");

            return;
        }
        
        static void PreInterpret()
        {
            // Stuff
            while (memoryIndex < 0) memoryIndex += memorySize;
            if(memoryIndex >= memorySize) memoryIndex %= memorySize;
        }
        static void Interpret()
        {
            switch (instructions[instructionIndex])
            {
                case '>':
                    memoryIndex++;
                    break;

                case '<':
                    memoryIndex--;
                    break;

                case '+':
                    memory[memoryIndex]++;
                    break;

                case '-':
                    memory[memoryIndex]--;
                    break;

                case '.':
                    Console.Write((char)memory[memoryIndex]);
                    break;

                case ',':
                    memory[memoryIndex] = (scalar)Console.ReadKey(false).KeyChar;
                    break;

                case '[':
                    if (memory[memoryIndex] == 0)
                    {
                        int indices = 0;
                        do
                        {
                            if (instructions[instructionIndex] == '[') indices++;
                       else if (instructions[instructionIndex] == ']') indices--;
                            instructionIndex++;
                        }
                        while (indices > 0);
                        instructionIndex--;
                    }
                    break;

                case ']':
                    if (memory[memoryIndex] != 0)
                    {
                        int indices = 0;
                        do
                        {
                            if (instructions[instructionIndex] == '[') indices--;
                       else if (instructions[instructionIndex] == ']') indices++;
                            instructionIndex--;
                        }
                        while (indices > 0);
                        instructionIndex++;
                    }
                    break;
            }
        }
    }
}
