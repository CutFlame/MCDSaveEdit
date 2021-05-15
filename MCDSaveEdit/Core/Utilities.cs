using System;
using System.Collections;
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
            var options = new JsonSerializerOptions() { WriteIndented = true };
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(unPrettyJson);
            return JsonSerializer.Serialize(jsonElement, options);
        }

        public static IEnumerable<T> dropLast<T>(this IEnumerable<T> enumerable, int numberToDropFromEnd)
        {
            var count = enumerable.Count();
            return enumerable.Take(count - numberToDropFromEnd);
        }

        public static IEnumerable<T> deepClone<T>(this IEnumerable<T> enumerable) where T:ICloneable
        {
            return enumerable.Select(element => element.Clone()).Cast<T>();
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
