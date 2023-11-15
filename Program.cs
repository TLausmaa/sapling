void RunTests()
{
    var tests = new TokenizerTests();
    tests.RunAll();
}

void CompileTarget(CompileOptions options)
{
    var content = FileReader.ReadFile(options.Filename);
    var tokenizer = new Tokenizer();
    var tokens = tokenizer.Tokenize(content);
    if (options.PrintDebug) {
        TokenPrinter.Print(tokens);
    }

    var ast = new Ast();
    ast.Build(tokens);
    if (options.PrintDebug) {
        AstPrinter.Print(ast);
    }

    var codeGen = new CodeGenerator();
    var output = codeGen.Generate(ast);
    if (options.PrintDebug) {
        codeGen.DebugPrint(output);
    }

    if (options.PrintOutput) {
        Console.WriteLine(output.ToString());
    }
}

/*
    Different workloads are:
    1. Compile a file through complete pipeline: $ program <filename>
        --debug:  print debug info
        --output: print output
    2. Run all tests                             $ program --test
*/
void Run()
{
    var opts = Environment.GetCommandLineArgs();

    if (opts.Contains("--test"))
    {
        RunTests();
        return;
    }

    var filename = opts.Length >= 2 ? opts[1] : "TestSource/function_call_with_args.spl";
    
    CompileTarget(new CompileOptions() {
        Filename = filename,
        PrintDebug = opts.Contains("--debug"),
        PrintOutput = opts.Contains("--output")
    });
}

Run();

record struct CompileOptions
{
    public string Filename { get; init; }
    public bool PrintDebug { get; init; }
    public bool PrintOutput { get; init; }
}