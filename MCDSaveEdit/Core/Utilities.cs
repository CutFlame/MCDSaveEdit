using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCDSaveEdit
{
    public static class Utilities
    {
        public static string prettyJson(string unPrettyJson)
        {
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);
            var options = new JsonSerializerOptions() { WriteIndented = true };
            return JsonSerializer.Serialize(jsonElement, options);
        }

        public static Dictionary<T1, T2> concatMissingFrom<T1, T2>(this Dictionary<T1, T2> dictA, Dictionary<T1, T2> dictB)
        {
            return dictA.Concat(dictB.Where(kvp => !dictA.ContainsKey(kvp.Key))).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
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
    }
}
