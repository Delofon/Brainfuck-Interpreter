using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainfuck_Interpreter
{
    class Program
    {
        const uint memorySize = 128; // Gotta be limited on memory bitch

        static uint[] memory;
        static uint   memoryIndex;

        static List<char> instructions;
        static int instructionIndex;

        static FileStream   file;
        static StreamReader reader;

        static int indices;

        static void Main(string[] args)
        {
            #region Program Load

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

            #region Brainfuck Interpretation

            instructions = new List<char>();

            do
            {
                addOp(instructions);
            }
            while (!reader.EndOfStream);

            if (indices != 0)
            {
                Console.WriteLine("The amount of received '[' does not match the amount of received ']', detected mismatch of {0} (']' - '[')", indices);
                Environment.Exit(3);
            }

            #endregion

            reader.Close();
            file.Close();

            #region Brainfuck Runtime

            memory = new uint[memorySize];

            instructionIndex = 0;

            do
            {
                PreInterpret();
                Interpret(instructions[instructionIndex]);
                instructionIndex++;
            }
            while (instructionIndex != instructions.Count - 1);

            #endregion

            Console.WriteLine();

            return;
        }
        
        static void PreInterpret()
        {
            // Stuff
            memoryIndex %= memorySize;
        }
        static void Interpret(char action)
        {
            if(!char.IsWhiteSpace(action))
            {
                switch(action)
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
                        if(memory[memoryIndex] == 0)
                        {
                            int indices = 0;
                            do
                            {
                                if (instructions[instructionIndex] == '[') indices++;
                                if (instructions[instructionIndex] == ']') indices--;
                                instructionIndex++;
                            }
                            while (indices != 0);
                        }
                        break;

                    case ']':
                        if (memory[memoryIndex] != 0)
                        {
                            int indices = 0;
                            do
                            {
                                if (instructions[instructionIndex] == '[') indices--;
                                if (instructions[instructionIndex] == ']') indices++;
                                instructionIndex--;
                            }
                            while (indices != 0);
                        }
                        break;
                }
            }
        }

        static void addOp(List<char> operations)
        {
            char operation = (char)reader.Read();

            if (!char.IsWhiteSpace(operation))
            {
                switch ((char)reader.Read())
                {
                    case '>':
                    case '<':
                    case '+':
                    case '-':
                    case '.':
                    case ',':
                        operations.Add(operation);
                        break;

                    case '[':
                        operations.Add(operation);
                        indices++;
                        break;

                    case ']':
                        operations.Add(operation);
                        indices--;
                        break;

                    default:
                        catchErrorWrongOp(operation);
                        return;
                }
            }
        }

        static void catchErrorWrongOp(char action)
        {
            Console.WriteLine("Unknown operator {1} detected at position {0}. Interpretation terminated.", instructionIndex, action);
            Environment.Exit(2);
        }
    }
}
