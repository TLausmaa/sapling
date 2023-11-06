class TokenizerTests
{
    List<(string, List<Token>)> GetTestCases()
    {
        var testCases = new List<(string, List<Token>)>() {
            ("TestSource/hello_world.spl", new List<Token>() {
                new() { Type = TokenType.FnDecl, Value = "fn" },
                new() { Type = TokenType.Identifier, Value = "helloworld" },
                new() { Type = TokenType.LeftBrace, Value = "{" },
                new() { Type = TokenType.Identifier, Value = "print" },
                new() { Type = TokenType.LeftParenthesis, Value = "(" },
                new() { Type = TokenType.String, Value = @"""hello world""" },
                new() { Type = TokenType.RightParenthesis, Value = ")" },
                new() { Type = TokenType.RightBrace, Value = "}" },
            }),
            ("TestSource/variables.spl", new List<Token>() {
                new() { Type = TokenType.VariableDecl, Value = "let" },
                new() { Type = TokenType.Identifier, Value = "arr" },
                new() { Type = TokenType.Operator, Value = "=" },
                new() { Type = TokenType.LeftBracket, Value = "[" },
                new() { Type = TokenType.Number, Value = "1" },
                new() { Type = TokenType.Number, Value = "2" },
                new() { Type = TokenType.Number, Value = "3" },
                new() { Type = TokenType.RightBracket, Value = "]" },
                new() { Type = TokenType.VariableDecl, Value = "let" },
                new() { Type = TokenType.Identifier, Value = "instance_name" },
                new() { Type = TokenType.Operator, Value = "=" },
                new() { Type = TokenType.String, Value = @"""abc123""" },
                new() { Type = TokenType.VariableDecl, Value = "let" },
                new() { Type = TokenType.Identifier, Value = "index" },
                new() { Type = TokenType.Operator, Value = "=" },
                new() { Type = TokenType.Number, Value = "34" },
            }),
            ("TestSource/read_file.spl", new List<Token>() {
                new() { Type = TokenType.String, Value = @"""sapling""" },
                new() { Type = TokenType.LeftBrace, Value = "{" },
                new() { Type = TokenType.Identifier, Value = "readfile" },
                new() { Type = TokenType.RightBrace, Value = "}" },
                new() { Type = TokenType.VariableDecl, Value = "let" },
                new() { Type = TokenType.Identifier, Value = "content" },
                new() { Type = TokenType.Operator, Value = "=" },
                new() { Type = TokenType.Identifier, Value = "readfile" },
                new() { Type = TokenType.LeftParenthesis, Value = "(" },
                new() { Type = TokenType.String, Value = @"""hello_world.spl""" },
                new() { Type = TokenType.RightParenthesis, Value = ")" },
                new() { Type = TokenType.Identifier, Value = "print" },
                new() { Type = TokenType.LeftParenthesis, Value = "(" },
                new() { Type = TokenType.Identifier, Value = "content" },
                new() { Type = TokenType.RightParenthesis, Value = ")" },
            })
        };
        return testCases;
    }

    void AssertTokenizerResult(string filename, List<Token> expected)
    {
        var tokenizer = new Tokenizer();
        var tokens = tokenizer.Tokenize(FileReader.ReadFile(filename));
        Assert.AreListsEqual(expected, tokens);
    }

    public void RunAll()
    {
        foreach (var testCase in GetTestCases())
        {
            Console.Write($"Running test {testCase.Item1}");
            AssertTokenizerResult(testCase.Item1, testCase.Item2);
            Console.WriteLine(" - OK");
        }
    }
}
