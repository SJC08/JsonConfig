using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Asjc.JsonConfig
{
    public sealed class JsonConfigOptions
    {
        public JsonConfigOptions() { }

        public JsonConfigOptions(JsonConfigOptions options)
        {
            CreateNew = options.CreateNew;
            SaveNew = options.SaveNew;
            SerializerOptions = options.SerializerOptions;
        }

        public bool CreateNew { get; set; } = true;

        public bool SaveNew { get; set; } = true;

        public JsonSerializerOptions SerializerOptions { get; set; } = new()
        {
            AllowTrailingCommas = true,
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals | JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true
        };
    }
}
