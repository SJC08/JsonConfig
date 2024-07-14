using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Asjc.Extensions.TryHelper;

namespace Asjc.JsonConfig
{
    public abstract class JsonConfig
    {
        /// <summary>
        /// The path to the config file.
        /// </summary>
        [JsonIgnore]
        public string Path { get; set; }

        protected virtual string DefaultPath => $"{GetType().Name}.json";

        [JsonIgnore]
        public JsonConfigOptions Options { get; set; }

        protected virtual JsonConfigOptions DefaultOptions => GlobalOptions;

        public static JsonConfigOptions GlobalOptions { get; set; } = new JsonConfigOptions();

        /// <summary>
        /// Gets the JSON string representation of the current object.
        /// </summary>
        [JsonIgnore]
        public string Json => JsonSerializer.Serialize(this, GetType());

        /// <summary>
        /// Occurs when loading a config from a file.
        /// </summary>
        public event Action<JsonConfig>? Reading;

        /// <summary>
        /// Occurs when creating a new config.
        /// </summary>
        public event Action<JsonConfig>? Creating;

        /// <summary>
        /// Occurs when a config is loaded.
        /// </summary>
        public event Action<JsonConfig>? AfterLoad;

        /// <summary>
        /// Occurs before saving the config.
        /// </summary>
        public event Action<JsonConfig>? BeforeSave;

        /// <summary>
        /// Occurs after saving the config.
        /// </summary>
        public event Action<JsonConfig>? AfterSave;

        protected virtual void OnReading() => Reading?.Invoke(this);

        protected virtual void OnCreating() => Creating?.Invoke(this);

        protected virtual void OnAfterLoad() => AfterLoad?.Invoke(this);

        protected virtual void OnBeforeSave() => BeforeSave?.Invoke(this);

        protected virtual void OnAfterSave() => AfterSave?.Invoke(this);

        public static T? Load<T>() where T : JsonConfig, new() => Load<T>(new T().DefaultPath, new T().DefaultOptions);

        public static T? Load<T>(string path) where T : JsonConfig, new() => Load<T>(path, new T().DefaultOptions);

        public static T? Load<T>(JsonConfigOptions options) where T : JsonConfig, new() => Load<T>(new T().DefaultPath, options);

        public static T? Load<T>(string path, JsonConfigOptions options) where T : JsonConfig, new()
        {
            T? config = default;
            if (File.Exists(path))
                config = Read<T>(path, options);
            else if (options.CreateNew)
                config = Create<T>(path, options);
            config?.OnAfterLoad();
            return config;
        }

        public static bool TryLoad<T>(out T? config) where T : JsonConfig, new() => Try(() => Load<T>(), out config);

        public static bool TryLoad<T>(string path, out T? config) where T : JsonConfig, new() => Try(() => Load<T>(path), out config);

        public static bool TryLoad<T>(JsonConfigOptions options, out T? config) where T : JsonConfig, new() => Try(() => Load<T>(options), out config);

        public static bool TryLoad<T>(string path, JsonConfigOptions options, out T? config) where T : JsonConfig, new() => Try(() => Load<T>(path, options), out config);

        public static T? TryLoad<T>() where T : JsonConfig, new() => Try(() => Load<T>());

        public static T? TryLoad<T>(string path) where T : JsonConfig, new() => Try(() => Load<T>(path));

        public static T? TryLoad<T>(JsonConfigOptions options) where T : JsonConfig, new() => Try(() => Load<T>(options));

        public static T? TryLoad<T>(string path, JsonConfigOptions options) where T : JsonConfig, new() => Try(() => Load<T>(path, options));

        public static T SafeLoad<T>() where T : JsonConfig, new() => TryLoad<T>() ?? Create<T>();

        public static T SafeLoad<T>(string path) where T : JsonConfig, new() => TryLoad<T>(path) ?? Create<T>(path);

        public static T SafeLoad<T>(JsonConfigOptions options) where T : JsonConfig, new() => TryLoad<T>(options) ?? Create<T>(options);

        public static T SafeLoad<T>(string path, JsonConfigOptions options) where T : JsonConfig, new() => TryLoad<T>(path, options) ?? Create<T>(path, options);

        public static T? Read<T>() where T : JsonConfig, new() => Read<T>(new T().DefaultPath, new T().DefaultOptions);

        public static T? Read<T>(string path) where T : JsonConfig, new() => Read<T>(path, new T().DefaultOptions);

        public static T? Read<T>(JsonConfigOptions options) where T : JsonConfig, new() => Read<T>(new T().DefaultPath, options);

        public static T? Read<T>(string path, JsonConfigOptions options) where T : JsonConfig, new()
        {
            string json = File.ReadAllText(path);
            T? config = JsonSerializer.Deserialize<T>(json, options.SerializerOptions); // When deserializing "null", it returns null!
            if (config != null)
            {
                config.OnReading();
                config.Path = path;
                config.Options = options;
            }
            return config;
        }

        public static T Create<T>() where T : JsonConfig, new() => Create<T>(new T().DefaultPath, new T().DefaultOptions);

        public static T Create<T>(string path) where T : JsonConfig, new() => Create<T>(path, new T().DefaultOptions);

        public static T Create<T>(JsonConfigOptions options) where T : JsonConfig, new() => Create<T>(new T().DefaultPath, options);

        public static T Create<T>(string path, JsonConfigOptions options) where T : JsonConfig, new()
        {
            T config = new T();
            config.OnCreating();
            config.Path = path;
            config.Options = options;
            if (options.SaveNew)
                config.Save();
            return config;
        }

        public void Save() => Save(Path, Options);

        public void Save(string path) => Save(path, Options);

        public void Save(JsonConfigOptions options) => Save(Path, options);

        public void Save(string path, JsonConfigOptions options)
        {
            OnBeforeSave();
            string json = JsonSerializer.Serialize(this, GetType(), options.SerializerOptions);
            File.WriteAllText(path, json);
            OnAfterSave();
        }

        public bool TrySave() => Try(() => Save());

        public bool TrySave(string path) => Try(() => Save(path));

        public bool TrySave(JsonConfigOptions options) => Try(() => Save(options));

        public bool TrySave(string path, JsonConfigOptions options) => Try(() => Save(path, options));

        public override string ToString() => Json;
    }
}
