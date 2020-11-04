using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCDSaveEdit
{
    public static class Utilities
    {
        public static string prettyJson(string unPrettyJson)
        {
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);
            return JsonSerializer.Serialize(jsonElement, options);
        }

        public static async Task<string> wgetAsync(string requestUriString)
        {
            try
            {
                var request = WebRequest.Create(requestUriString);
                using var response = await request.GetResponseAsync();
                using var dataStream = response.GetResponseStream();
                using var reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                response.Close();
                return responseFromServer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "{}";
        }

        public static string wget(string requestUriString)
        {
            var request = WebRequest.Create(requestUriString);
            using var response = request.GetResponse();
            using var dataStream = response.GetResponseStream();
            using var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            response.Close();
            return responseFromServer;
        }
    }
}
