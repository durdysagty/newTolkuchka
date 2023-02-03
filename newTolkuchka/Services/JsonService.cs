using newTolkuchka.Models;
using System.Text.Json;

namespace newTolkuchka.Services
{
    public static class JsonService
    {
        public static TValue Deserialize<TValue>(string json)
        {
            TValue value = JsonSerializer.Deserialize<TValue>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return value;
        }
    }
}
