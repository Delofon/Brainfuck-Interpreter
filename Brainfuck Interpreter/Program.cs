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
        static void Main(string[] args)
        {
            uint[] memory = new uint[128];
            uint memoryIndex = 0;

            FileStream file;
            StreamReader reader;

            Console.WriteLine("Please enter path to the file");
            string filePath = Console.ReadLine();

            try
            {
                file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch
            {
                Console.WriteLine("Wrong path. Interpretation terminated.");
                Environment.Exit(-1);
                return;
            }
            
            reader = new StreamReader(file);

            List<char> operations = new List<char>();

            int curSymbol = 0;

            do
            {
                addOp(operations, reader, ref curSymbol);
            }
            while (!reader.EndOfStream);

            reader.Close();
            file.Close();

            curSymbol = 0;

            List<int> leftIndices = new List<int>();
            List<int> rightIndices = new List<int>();

            do
            {
                Interpret(ref memory, ref memoryIndex, leftIndices, rightIndices, operations[curSymbol], curSymbol);
                curSymbol++;
            }
            while (curSymbol != operations.Count - 1);

            Console.WriteLine();

            return;
        }

        static void Interpret(ref uint[] memory, ref int memoryIndex, List<int> leftIndices, List<int> rightIndices, char action, int curSymbol)
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
                        Console.WriteLine("Sorry! Operator [ not implemented yet!");
                        break;

                    case ']':
                        Console.WriteLine("Sorry! Operator ] not implemented yet!");
                        break;
                }
            }
        }

        static void addOp(List<char> operations, StreamReader reader, ref int curSymbol)
        {
            char operation = (char)reader.Read();

            if (!char.IsWhiteSpace(operation))
            {
                switch ((char)reader.Read())
                {
                    case '>':
                        operations.Add(operation);
                        break;

                    case '<':
                        operations.Add(operation);
                        break;

                    case '+':
                        operations.Add(operation);
                        break;

                    case '-':
                        operations.Add(operation);
                        break;

                    case '.':
                        operations.Add(operation);
                        break;

                    case ',':
                        operations.Add(operation);
                        break;

                    case '[':
                        operations.Add(operation);
                        break;

                    case ']':
                        operations.Add(operation);
                        break;

                    default:
                        catchErrorWrongOp(curSymbol, operation);
                        return;
                }
            }
        }

        static void catchErrorWrongOp(int curSymbol, char action)
        {
            Console.WriteLine("Unknown operator {1} detected at position {0}. Interpretation terminated.", curSymbol, action);
            Environment.Exit(-1);
        }
    }
}
