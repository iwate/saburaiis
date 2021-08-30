using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SaburaIIS.Json
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (TimeSpan.TryParse(reader.GetString(), out var time))
                return time;

            return default;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
