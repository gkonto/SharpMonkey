using static System.Console;
using lexer;

namespace repl
{
    public class Repl
    {
        private const string prompt = ">> ";

        public static void start()
        {
            while (true) {
                Write(prompt);
                string input = ReadLine();
                
                if (input.Length == 0) {
                    break;
                }
                var l = new Lexer(input);

                for (var tok = l.NextToken(); tok.Type != token.TokenType.EOF; tok = l.NextToken()) {
                    WriteLine($"Type: {tok.Type} - Literal: {tok.Literal}");
                }
            }
        }
    }
}