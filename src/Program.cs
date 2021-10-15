using System;
using static System.Console;

namespace monkey
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            WriteLine("Hello friend! This is the Monkey programming language!");
            WriteLine("Feel free to type in commands");
            Repl.start();
        }
    }
}
