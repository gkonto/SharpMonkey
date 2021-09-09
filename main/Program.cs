using System;
using static System.Console;
using repl;

namespace main
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Hello friend! This is the Monkey programming language!");
            WriteLine("Feel free to type in commands");
            repl.Repl.start();
        }
    }
}
