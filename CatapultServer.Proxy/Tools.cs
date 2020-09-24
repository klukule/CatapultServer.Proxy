using System.IO;
using Newtonsoft.Json;

namespace CatapultServer.Proxy
{
    public static class Tools
    {
        /// <summary>
        /// Hacky way to pretty print object in JSON representation
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonPrettify(object obj)
        {
            // TODO: Optimize this bs
            using var stringReader = new StringReader(JsonConvert.SerializeObject(obj));
            using var stringWriter = new StringWriter();
            var jsonReader = new JsonTextReader(stringReader);
            var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }
    }
}
