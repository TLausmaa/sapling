using System.Diagnostics;

void RunTests()
{
    var tests = new TokenizerTests();
    tests.RunAll();
}

string RunCommandWithBash(string command)
{
    var psi = new ProcessStartInfo();
    psi.FileName = "/bin/bash";
    psi.Arguments = $"-c {command}";
    psi.RedirectStandardOutput = true;
    psi.UseShellExecute = false;
    psi.CreateNoWindow = true;

    using var process = Process.Start(psi);

    process.WaitForExit();

    var output = process.StandardOutput.ReadToEnd();

    return output;
}

void CompileTarget(string filename, bool printDebugInfo = false, bool printOutput = false)
{
    var content = FileReader.ReadFile(filename);
    var tokenizer = new Tokenizer();
    var tokens = tokenizer.Tokenize(content);
    if (printDebugInfo) {
        TokenPrinter.Print(tokens);
    }

    var ast = new Ast();
    ast.Build(tokens);
    if (printDebugInfo) {
        AstPrinter.Print(ast);
    }

    var codeGen = new CodeGenerator();
    var output = codeGen.Generate(ast);
    if (printDebugInfo) {
        codeGen.DebugPrint(output);
    }

    if (printOutput) {
        Console.WriteLine(output.ToString());
    }
    // var output = RunCommandWithBash("echo 'hi'");
    // Console.WriteLine(output);
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
    var opts = Environment.GetCommandLineArgs();
    var printDebug = false;
    var printOutput = false;
    if (opts.Length >= 3)
    {
        printDebug = opts[2] == "--debug";
        printOutput = opts[2] == "--output";
    }

    CompileTarget("TestSource/hello_world.spl", printDebug, printOutput);
}

Run();