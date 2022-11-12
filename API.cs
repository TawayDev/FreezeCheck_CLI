using System.Net;
using RestSharp;
using static NTKZP_CLI.Util;
using static RestSharp.Method;

namespace NTKZP_CLI; 

public class API {
    public static bool CACHED = false;
    public static string CLASS_NAME;
    public static int? PC_ID;
    
    private static readonly string API_URL = "http://localhost:8080/";
    private static int TIMEOUT = 5000;
    public static bool InformAPI(String TIME) {
        // TODO: Post destination:

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
            request.AddParameter("TIME",$"{TIME}", ParameterType.GetOrPost);

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
}