using System.Text.Json;
using System.Text.Json.Serialization;

class TokenizerTests
{
    class TokenizerValidationCase
    {
        public List<Token> ExpectedTokens { get; set; } = new List<Token>();
    }

    public List<(string validationFile, string sourceFile)> GetTestFiles()
    {
        var sourceFiles = Directory.GetFiles("TestSource").Where(f => f.EndsWith(".spl")).ToList();
        var validationFiles = Directory.GetFiles("TestSource").Where(f => f.EndsWith(".json")).ToList();
        return validationFiles
            .Select(f => (f, sourceFiles.First(sf => sf.Contains(Path.GetFileNameWithoutExtension(f)))))
            .ToList();
    }

    public void RunAll()
    {
        var tokenizer = new Tokenizer();
        var testCases = GetTestFiles();

        foreach (var test in testCases)
        {
            Console.Write($"Running test {test.sourceFile}");
            
            using FileStream stream = File.OpenRead(test.validationFile);
            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = 
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            TokenizerValidationCase? validationCase = null;
            try {
                validationCase = JsonSerializer.Deserialize<TokenizerValidationCase>(stream, serializerOptions);
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            var tokens = tokenizer.Tokenize(FileReader.ReadFile(test.sourceFile));
            Assert.AreListsEqual(validationCase!.ExpectedTokens, tokens);   

            Console.WriteLine(" - OK");
        }
    }
}
