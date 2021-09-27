using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SaburaIIS.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SaburaIIS.Tests
{
    public class Helpers
    {
        private static readonly JsonSerializerOptions options;
        
        static Helpers()
        {
            options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new TimeSpanConverter());
        }

        public static T Clone<T>(T obj) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj, options), options);
        public static string ToJson<T>(T obj) => JsonSerializer.Serialize(obj, options);

        public static ILogger<T> CreateTestLogger<T>() => ((ILoggerFactory)new NullLoggerFactory()).CreateLogger<T>();
    }
}
