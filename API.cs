using System.Net;
using RestSharp;
using static NTKZP_CLI.Util;
using static RestSharp.Method;

namespace NTKZP_CLI; 

public class API {
    private static bool CACHED = false;
    private static readonly string API_URL = "http://localhost:8080/";
    private static readonly string ENDPOINT = "";
    private static string CLASS_NAME;
    private static int? PC_ID;
    private static int TIMEOUT = 5000;
    public static bool InformAPI() {
        // TODO: Post destination:
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
        }
        catch (Exception e) {
            FancyPrint($"Failed to cache data. Reason: {e.Message}", LogLevel.ERROR);
            CACHED = false;
        }
        
        try {
            FancyPrint("Attempting POST request to API", LogLevel.WARNING);
            var client = new RestClient(API_URL);
            var request = new RestRequest();
            request.Timeout = TIMEOUT;

            // Content type is not required when adding parameters this way
            // This will also automatically UrlEncode the values
            request.AddHeader("Content-Type", Post);
            request.AddParameter("CLASS_NAME",CLASS_NAME, ParameterType.GetOrPost);
            request.AddParameter("PC_ID",PC_ID, ParameterType.GetOrPost);
            request.AddParameter("TIME",$"M{DateTime.Now.Month}D{DateTime.Now.Day}H{DateTime.Now.Hour}M{DateTime.Now.Minute}S{DateTime.Now.Second}", ParameterType.GetOrPost);

            RestResponse response = client.Post(request);
            // Check if response timed out
            if (response.StatusCode == HttpStatusCode.RequestTimeout) return false;
            // This code will execute if the request was successful
            FancyPrint($"[API - REPLY] {response.Content}", LogLevel.IMPORTANT);
            return true;
        } catch (Exception e) {
            FancyPrint($"An error occured while attempting to contact API. Reason: {e.Message}", LogLevel.ERROR);
            return false;
        }
    }
    
    public static bool GET() {
        return true;
    }
}