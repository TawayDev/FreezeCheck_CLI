using System.Diagnostics;
using Microsoft.Win32;
using static NTKZP_CLI.Util;
namespace NTKZP_CLI;
public class ToolwizCheck {
    // TTF stands for ToolwizTimeFreeze
    private static bool isRunningProcess;
    private static bool REGISTRY_RUN_EXISTS;
    private static bool REGISTRY_RUN_EXISTS_WITH_PATH;
    private static object? CURRENT_REGISTRY_PROTECT_MODE;
    private static object? CURRENT_REGISTRY_NEXT_BOOT_PROTECT;

    public static bool TTFCheck(){
        try {
            // Check if the process is running
            Process[] processlist = Process.GetProcesses();
            foreach(Process process in processlist){
                if (process.ProcessName.Contains("ToolwizTimeFreeze")) {
                    isRunningProcess = true;
                }
            }

            // Check if the registry is set correctly
            CURRENT_REGISTRY_PROTECT_MODE = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Toolwiz\\TimeFreezeNew", "CURRENT_PROTECT_MODE",null);
            CURRENT_REGISTRY_NEXT_BOOT_PROTECT = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Toolwiz\\TimeFreezeNew", "NEXT_BOOT_PROTECT",null);
            
            // Computer\HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run
            // "C:\Program Files\Toolwiz Time Freeze 2017\ToolwizTimeFreeze.exe" -autorun
            var RUN = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run", "ToolwizTimeFreeze",null);
            REGISTRY_RUN_EXISTS = RUN != null;
            
            // Checks if registry exists. If it does, it checks if the path is correct. If it does not then RUN is null.
            if(REGISTRY_RUN_EXISTS_WITH_PATH = RUN != null){
                REGISTRY_RUN_EXISTS_WITH_PATH = RUN.ToString().Contains("ToolwizTimeFreeze");
            } else {
                REGISTRY_RUN_EXISTS_WITH_PATH = false;
            }
            CURRENT_REGISTRY_PROTECT_MODE ??= 0;
            CURRENT_REGISTRY_NEXT_BOOT_PROTECT ??= 0;
            
            // Evaluate the results
            int result = 0;
            if(isRunningProcess) result += 1;
            if(REGISTRY_RUN_EXISTS) result += 1;
            if(REGISTRY_RUN_EXISTS_WITH_PATH) result += 1;
            if(CURRENT_REGISTRY_PROTECT_MODE.ToString() == "1") result += 1;
            if(CURRENT_REGISTRY_NEXT_BOOT_PROTECT.ToString() == "1") result += 1;
            // Convert result to percent
            int percent = (int) Math.Round((double) result / 5 * 100);
            bool passed = percent >= Program.REQUIRED_PERCENT;
            
            // CLI debug bullshit:
            FancyPrint("+------------------------------------------+");
            FancyPrint(" CURRENT_REGISTRY_PROTECT_MODE:      " + CURRENT_REGISTRY_PROTECT_MODE);
            FancyPrint(" CURRENT_REGISTRY_NEXT_BOOT_PROTECT: " + CURRENT_REGISTRY_NEXT_BOOT_PROTECT);
            FancyPrint(" REGISTRY_RUN_EXISTS:                " + REGISTRY_RUN_EXISTS);
            FancyPrint(" REGISTRY_RUN_EXISTS_WITH_PATH:      " + REGISTRY_RUN_EXISTS_WITH_PATH);
            FancyPrint(" isRunningProcess:                   " + isRunningProcess);
            FancyPrint("+------------------------------------------+");
            FancyPrint("RESULT: " + result + "/5 " + "(" + percent + "%)");
            FancyPrint("PASSED: " + passed);
            return passed;
        } catch (Exception e) {
            FancyPrint(e.Message, LogLevel.ERROR);
            return true;
        }
    }
}