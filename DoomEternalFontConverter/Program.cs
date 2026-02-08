using DoomEternalFontConverter;
using System.Diagnostics.Tracing;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLower();
        string inputFile = args[1];
        string outputFile = args[2];

        try
        {
            if (command == "tojson")
            {
                Console.WriteLine($"Reading binary file: {inputFile}...");
                var fontInfo = FontBinaryHandler.Read(inputFile);
                string json = fontInfo.ToJson();
                File.WriteAllText(outputFile, json);
                Console.WriteLine($"Successfully converted to JSON: {outputFile}");
            }
            else if (command == "tobin")
            {
                Console.WriteLine($"Reading JSON file: {inputFile}...");

                if (!File.Exists(inputFile))
                {
                    throw new FileNotFoundException("JSON file not found");
                }
                string json = File.ReadAllText(inputFile);
                var fontInfo = json.FromJson();
                if (fontInfo == null)
                {
                    throw new Exception("Failed to parse JSON.");
                }
                FontBinaryHandler.Write(outputFile, fontInfo);
                Console.WriteLine($"Successfully converted to Binary: {outputFile}");
            }
           else if (command == "generate")
           {
                string originalFont = inputFile;
                string fntPath = outputFile;
                Console.WriteLine($"Reading binary file: {inputFile}...");
                var fontInfo = FontBinaryHandler.Read(inputFile);

                Console.WriteLine($"Generating binary font from BMFont to binary from: {fntPath}...");
                var generatedFontInfo = FontProcessor.GenerateFontFromBmFont(fntPath, fontInfo.MaterialName);
                string savePath = args[3];
                    /* Path.Combine(Path.GetDirectoryName(fntPath) ?? "", $"{Path.GetFileNameWithoutExtension(fntPath)}");*/
                Console.WriteLine($"Writing generated binary font to: {savePath}...");
                FontBinaryHandler.Write(savePath, generatedFontInfo);
            }
            else
            {
                Console.WriteLine($"Unknown command: {command}");
                PrintUsage();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR]: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Details: {ex.InnerException.Message}");
            }
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("\nDoom Eternal Font Converter");
        Console.WriteLine("Usage:");
        Console.WriteLine("  To JSON:   DoomEternalFontConverter.exe tojson <input.font> <output.json>");
        Console.WriteLine("  To Binary: DoomEternalFontConverter.exe tobin <input.json> <output.font>");
        Console.WriteLine("  Generate Binary from BMFont: DoomEternalFontConverter.exe generate <original.font> <input.fnt> <output.font>");
    }
}