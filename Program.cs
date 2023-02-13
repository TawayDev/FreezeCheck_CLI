using static NTKZP_CLI.ToolwizCheck;
using static NTKZP_CLI.API;
using static NTKZP_CLI.Util;
public class Program {
    // Debug thingies
    public static readonly bool PRINT_DEBUG = true;
    private static readonly bool EXIT_AFTER_POST = false;
    
    // QOL
    public static bool PRINT_REG_ONLY_IF_CHANGED = true;
    
    // Controls how many out of 5 checks need to fail in order for the program to contact API
    public static int REQUIRED_PERCENT = 100;
    
    // API thingies
    private static bool POSTED = false;
    private static int RETRY_POST_AFTER_FAIL_MILLIS = 5000;
    private static int RETRY_POST_AFTER_SUCCESS_MILLIS = 3600000;
    private static int POST_ATTEMPT_COUNT = 0;
    private static DateTime POST_TIME;
    public static void Main(string[] args) {

        // This line of code is really funny to me... I don't know why
        // Marshal.AllocCoTaskMem(unchecked((int) Double.PositiveInfinity));

        // Set self as high priority
        System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
        
        // TODO: Move caching to init when you move CLI to WS
        // Cache data because some fucker might delete config and we might not send correct data to API
        try {
            if (!CACHED) {
                FancyPrint("Caching deserialized json data");
                ClassJSON classJson = GetClassJSON();
                CLASS_NAME = classJson.CLASS_NAME;
                PC_ID = classJson.PC_ID;
                CACHED = true;
            }
        } catch (Exception e) {
            FancyPrint($"Failed to cache data. Reason: {e.Message}", LogLevel.ERROR);
            CACHED = false;
        }

        bool PASSED = TTFCheck();
        if (!PASSED && !POSTED) {
            POST_ATTEMPT_COUNT++;
            // Save first POST attempt time so it will not change every at POST attempt
            if(POST_ATTEMPT_COUNT == 1 ) POST_TIME = DateTime.Now;
            // POST:
            POSTED = InformAPI(POST_TIME.ToString(@"yyyy-MM-dd HH_mm_ss"));
            // infinite loop thingy:
            if (!POSTED) {
                FancyPrint($"[ATTEMPT: {POST_ATTEMPT_COUNT}][POSTED: {POSTED}]> Failed to post to API", LogLevel.WARNING);
                FancyPrint($"POST request will be attempted again in {TimeSpan.FromMilliseconds(RETRY_POST_AFTER_FAIL_MILLIS).ToString(@"hh\:mm\:ss")}");
                Thread.Sleep(RETRY_POST_AFTER_FAIL_MILLIS);
                POST_ATTEMPT_COUNT++;
                Main(new string[] {});
            }
            if (POSTED) {
                FancyPrint($"[ATTEMPT: {POST_ATTEMPT_COUNT}][POSTED: {POSTED}]> Successfully posted to API", LogLevel.SUCCESS);
                if (EXIT_AFTER_POST) {
                    FancyPrint("EXIT_AFTER_POST is set to true, exiting...", LogLevel.WARNING);
                    Environment.Exit(0);
                } else {
                    POST_ATTEMPT_COUNT = 0;
                    FancyPrint($"FreezeCheck will check registry and processes again in {TimeSpan.FromMilliseconds(RETRY_POST_AFTER_SUCCESS_MILLIS).ToString(@"hh\:mm\:ss")}");
                    Thread.Sleep(RETRY_POST_AFTER_SUCCESS_MILLIS);
                    Main(new string[] { });
                }
            }
        }
        FancyPrint($"Passed: {PASSED}, Posted: {POSTED}");
        Thread.Sleep(RETRY_POST_AFTER_FAIL_MILLIS);
        Main(new string[] { });
        // ReSharper disable once FunctionNeverReturns
    }
}