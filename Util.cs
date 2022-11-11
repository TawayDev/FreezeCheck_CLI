using System.Text.Json;

namespace NTKZP_CLI; 

public class Util {
    private static ConsoleColor defaultColor = ConsoleColor.Gray;
    private static ConsoleColor secondaryColor = ConsoleColor.White;
    private static int counter = 0;

    public enum LogLevel {
        INFO,
        WARNING = ConsoleColor.Yellow,
        ERROR = ConsoleColor.Red,
        SUCCESS = ConsoleColor.Green,
        IMPORTANT = ConsoleColor.Magenta
    }

    public static void FancyPrint(string text, LogLevel level = LogLevel.INFO) {
        if (!Program.PRINT_DEBUG) return;

        switch (level) {
            case LogLevel.INFO:
                if (counter == 0) {
                    Console.ForegroundColor = defaultColor;
                    Console.WriteLine($"[INFO] {text}");
                    counter = 1;
                } else {
                    Console.ForegroundColor = secondaryColor;
                    Console.WriteLine($"[INFO] {text}");
                    counter = 0;
                }
                break;
            default:
                Console.ForegroundColor = (ConsoleColor)level;
                Console.WriteLine($"[{level}] {text}");
                break;
        }

        // reset color so errors are not yellow.
        Console.ForegroundColor = ConsoleColor.White;
    }

    [Serializable]
    public class ClassJSON {
        public string? CLASS_NAME { get; set; }
        public int? PC_ID { get; set; }
    }
    
    public static ClassJSON GetClassJSON() {
        try {
            string fileName = "config.json";
            string jsonString = File.ReadAllText(fileName);
            ClassJSON classJson = JsonSerializer.Deserialize<ClassJSON>(jsonString)!;
            // FancyPrint($"Deserialized JSON: CLASS_NAME: {classJson.CLASS_NAME}, PC_ID: {classJson.PC_ID}", LogLevel.SUCCESS);
            
            return classJson;
        } catch (Exception e) {
            FancyPrint(e.Message, LogLevel.ERROR);
            ClassJSON cj = new ClassJSON();
            cj.PC_ID = 0;
            cj.CLASS_NAME = "ERROR";
            return cj;
        }
    }
}