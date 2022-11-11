using static NTKZP_CLI.ToolwizCheck;
using static NTKZP_CLI.API;
using static NTKZP_CLI.Util;
public class Program {
    public static int REQUIRED_PERCENT = 100;
    public static bool PRINT_DEBUG = true;
    public static bool PRINT_REG_ONLY_IF_CHANGED = true;
    private static bool POSTED = false;
    private static int RETRY_POST_AFTER_FAIL_MILLIS = 5000;
    private static int RETRY_POST_AFTER_SUCCESS_MILLIS = 3600000; //
    private static int POST_ATTEMPT_COUNT = 0;
    public static void Main(string[] args) {
        if (!TTFCheck() && !POSTED) {
            POST_ATTEMPT_COUNT++;

            POSTED = InformAPI();
        }
        
        // "For loop" thingy:
        if (POSTED) {
            FancyPrint($"[ATTEMPT: {POST_ATTEMPT_COUNT}][POSTED: {POSTED}]> Successfully posted to API", LogLevel.SUCCESS);
            FancyPrint($"POST request will be attempted again in {TimeSpan.FromMilliseconds(RETRY_POST_AFTER_SUCCESS_MILLIS).ToString(@"hh\:mm\:ss")}");
            Thread.Sleep(RETRY_POST_AFTER_SUCCESS_MILLIS);
            Main(new string[] {});
        } else {
            FancyPrint($"[ATTEMPT: {POST_ATTEMPT_COUNT}][POSTED: {POSTED}]> Failed to post to API", LogLevel.WARNING);
            FancyPrint($"POST request will be attempted again in {TimeSpan.FromMilliseconds(RETRY_POST_AFTER_FAIL_MILLIS).ToString(@"hh\:mm\:ss")}");
            Thread.Sleep(RETRY_POST_AFTER_FAIL_MILLIS);
            Main(new string[] {});
        }
        
        // ReSharper disable once FunctionNeverReturns
    }
}