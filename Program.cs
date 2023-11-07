void RunTests()
{
    var tests = new TokenizerTests();
    tests.RunAll();
}

void CompileTarget(string filename)
{
    var content = FileReader.ReadFile(filename);
    var tokenizer = new Tokenizer();
    var tokens = tokenizer.Tokenize(content);
    
    Console.WriteLine($"# Tokens are ({tokens.Count}):");
    Console.WriteLine("".PadRight(30, '='));
    foreach (var token in tokens)
    {
        var type = $"{token.Type}".PadRight(20, ' ');
        Console.WriteLine($"{type}{token.Value}");
    }

    var ast = new Ast();
    ast.Build(tokens);
    Console.WriteLine("");
    AstPrinter.Print(ast);
}

void Run()
{
    // var opts = Environment.GetCommandLineArgs();
    // if (opts.Length != 2)
    // {
    //     RunTests();
    //     return;
    // }
    
    // var filename = opts[1];
    CompileTarget("TestSource/hello_world.spl");
}

Run();