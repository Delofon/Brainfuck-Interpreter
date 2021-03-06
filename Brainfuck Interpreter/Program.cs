using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Brainfuck_Interpreter
{
    class Program
    {
        const uint memorySize = 128; // Gotta be limited on memory bitch

        static uint[] memory;
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
                file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
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

            int checkIndices = instructions.Count(x => x == ']') - instructions.Count(x => x == '[');
            if (checkIndices != 0)
            {
                Console.WriteLine("The amount of received '[' does not match the amount of received ']', detected mismatch of {0} (']' - '[')", checkIndices);
                Environment.Exit(2);
            }

            reader.Close();
            file.Close();

            #region Runtime

            memory = new uint[memorySize];

            instructionIndex = 0;
            memoryIndex = 0;

            for(instructionIndex = 0; instructionIndex < instructions.Length - 1; instructionIndex++)
            {
                //Thread.Sleep(1000);
                PreInterpret();
                Interpret();
            }

            #endregion

            Console.WriteLine();

            return;
        }
        
        static void PreInterpret()
        {
            // Stuff
            if (memoryIndex < 0) memoryIndex = 0;
            memoryIndex %= memorySize;
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
                    memory[memoryIndex] = Console.ReadKey(true).KeyChar;
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
                    }
                    break;
            }
        }
    }
}
