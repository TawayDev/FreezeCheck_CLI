using Flurl.Http;
using RestSharp;
using static NTKZP_CLI.Util;

namespace NTKZP_CLI; 

public class API {
    private static bool CACHED = false;
    private static readonly string API_URL = "http://localhost:8080/";
    private static string CLASS_NAME;
    private static int? PC_ID;
    public static bool InformAPI() {
        try {
            // TODO: Post destination:
            var client = new RestClient(API_URL);
            var request = new RestRequest("resource/{id}");
            
            // Cache data because some fucker might delete config and we could not send correct data to API
            if (!CACHED) {
                ClassJSON classJson = GetClassJSON();
                CLASS_NAME = classJson.CLASS_NAME;
                PC_ID = classJson.PC_ID;
                CACHED = true;
            }
            // Get cached data and post it to API
            request.AddParameter("CLASS_NAME", CLASS_NAME);
            request.AddParameter("PC_ID", $"{PC_ID}");
            request.AddParameter("TIME", DateTime.Now.ToString());

            // FancyPrint("CACHED: " + CACHED);
            // FancyPrint("CLASS_NAME: " + CLASS_NAME);
            // FancyPrint("PC_ID: " + PC_ID);
            // FancyPrint("TIME: " + DateTime.Now);
            
            var response = client.Post(request);
            return true;
        } catch (Exception e) {
            FancyPrint(e.Message, LogLevel.ERROR);
            return false;
        }
    }
    
    public static bool GET() {
        return true;
    }
}