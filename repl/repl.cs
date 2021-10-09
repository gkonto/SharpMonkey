using static System.Console;
using System.Collections.Generic;
using lexer;
using parser;
using ast;
using evaluator;
using menvironment;
using evalobject;

#nullable enable

namespace repl
{
    public class Repl
    {
        private const string prompt = ">> ";

        public static void printParserErrors(List<string> errors)
        {
            foreach (string msg in errors) {
                WriteLine($"\t{msg}");
            }
        }

        public static void start()
        {
            MEnvironment env = new MEnvironment();

            while (true) {
                Write(prompt);
                string input = ReadLine();
                
                if (input.Length == 0) {
                    break;
                }
                Lexer l = new Lexer(input);
                Parser p = new Parser(l);
                Program? program = p.ParseProgram();

                if (program != null) {
                    if (p.errors.Count != 0) {
                        printParserErrors(p.errors);
                        continue;
                    }
                }
                EvalObject evaluated = Evaluator.Eval(program, env);
                if (evaluated != null) {
                    WriteLine(evaluated.Inspect());
                }
            }
        }
    }
}