class TokenPrinter
{
    public static void Print(List<Token> tokens)
    {
        Console.WriteLine($"# Tokens are ({tokens.Count}):");
        Console.WriteLine("".PadRight(30, '='));
        foreach (var token in tokens)
        {
            var type = $"{token.Type}".PadRight(20, ' ');
            Console.WriteLine($"{(type + token.Value).PadRight(34, ' ')} @ {token.sourceLocation.Line}:{token.sourceLocation.Column}");
        }
        Console.WriteLine("");
    }
}
