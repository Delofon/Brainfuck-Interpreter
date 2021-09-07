using System;
using System.IO;
using System.Collections;
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

        static int checkIndices;

        static void Main(string[] args)
        {
            Console.InputEncoding  = Encoding.ASCII;
            Console.OutputEncoding = Encoding.ASCII;

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

            checkIndices = 0;

            do
            {
                AddInstruction();
            }
            while (!reader.EndOfStream);

            if (checkIndices != 0)
            {
                Console.WriteLine("The amount of received '[' does not match the amount of received ']', detected mismatch of {0} (']' - '[')", checkIndices);
                Environment.Exit(2);
            }
            #endregion

            reader.Close();
            file.Close();

            #region Brainfuck Runtime

            memory = new uint[memorySize];

            instructionIndex = 0;
            memoryIndex = 0;

            do
            {
                PreInterpret();
                Interpret();
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

        static void AddInstruction()
        {
            char instruction = (char)reader.Read();

            switch (instruction)
            {
                case '>':
                case '<':
                case '+':
                case '-':
                case '.':
                case ',':
                    instructions.Add(instruction);
                    break;

                case '[':
                    instructions.Add(instruction);
                    checkIndices++;
                    break;

                case ']':
                    instructions.Add(instruction);
                    checkIndices--;
                    break;
            }
        }
    }
}
