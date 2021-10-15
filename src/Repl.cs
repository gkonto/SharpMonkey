using static System.Console;
using System.Collections.Generic;


namespace monkey
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
                
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Lexer l = new Lexer(input);

                Parser p = new Parser(l);
                Program program = p.ParseProgram();
                watch.Stop();
                var parseMs = watch.ElapsedMilliseconds;

                if (program != null) {
                    if (p.errors.Count != 0) {
                        printParserErrors(p.errors);
                        continue;
                    }
                }
                watch = System.Diagnostics.Stopwatch.StartNew();
                EvalObject evaluated = Evaluator.Eval(program, env);
                watch.Stop();
                var evalMs = watch.ElapsedMilliseconds;

                if (evaluated != null) { 
                    WriteLine(evaluated.Inspect());
                }
                WriteLine($"\nTime Elapsed (ms)\n============\nParse: {parseMs}ms Eval: {evalMs}ms Total: {parseMs + evalMs}ms\n");
            }
        }
    }
}
