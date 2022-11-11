using static NTKZP_CLI.ToolwizCheck;
using static NTKZP_CLI.API;
public class Program {
    public static int REQUIRED_PERCENT = 100;
    public static bool PRINT_DEBUG = true;
    public static void Main(string[] args) {
        if (!TTFCheck()) {
            InformAPI();
        }
    }
}