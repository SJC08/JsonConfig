using System.Text.Json;
using System.Text.Json.Serialization;

namespace Asjc.JsonConfig
{
    public abstract class JsonConfig
    {
        [JsonIgnore]
        public string Path { get; set; }

        protected virtual string DefaultPath => $"{GetType().Name}.json";

        [JsonIgnore]
        public JsonConfigOptions? Options { get; set; }

        protected virtual JsonConfigOptions? DefaultOptions => null;

        public static JsonConfigOptions GlobalOptions { get; set; } = new();

        public event Action<JsonConfig>? Read;

        public event Action<JsonConfig>? Create;

        public event Action<JsonConfig>? AfterLoad;

        public event Action<JsonConfig>? BeforeSave;

        public event Action<JsonConfig>? AfterSave;

        protected virtual void OnRead() => Read?.Invoke(this);

        protected virtual void OnCreate() => Create?.Invoke(this);

        protected virtual void OnAfterLoad() => AfterLoad?.Invoke(this);

        protected virtual void OnBeforeSave() => BeforeSave?.Invoke(this);

        protected virtual void OnAfterSave() => AfterSave?.Invoke(this);

        public static T? Load<T>() where T : JsonConfig, new()
        {
            return Load<T>(new T().DefaultPath, new T().DefaultOptions);
        }

        public static T? Load<T>(string path) where T : JsonConfig, new()
        {
            return Load<T>(path, new T().DefaultOptions);
        }

        public static T? Load<T>(JsonConfigOptions? options) where T : JsonConfig, new()
        {
            return Load<T>(new T().DefaultPath, options);
        }

        public static T? Load<T>(string path, JsonConfigOptions? options) where T : JsonConfig, new()
        {
            var jco = options ?? GlobalOptions;
            T? config = default;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                config = JsonSerializer.Deserialize<T>(json, jco.SerializerOptions); // When deserializing "null", it returns null!
                if (config != null)
                {
                    config.OnRead();
                    config.Path = path;
                    config.Options = options;
                }
            }
            else if (jco.CreateNew)
            {
                config = new()
                {
                    Path = path,
                    Options = options
                };
                config.OnCreate();
                if (jco.SaveNew)
                    config.Save();
            }
            config?.OnAfterLoad();
            return config;
        }

        public static bool TryLoad<T>(out T? config) where T : JsonConfig, new()
        {
            return TryLoad(new T().DefaultPath, new T().DefaultOptions, out config);
        }

        public static bool TryLoad<T>(string path, out T? config) where T : JsonConfig, new()
        {
            return TryLoad(path, new T().DefaultOptions, out config);
        }

        public static bool TryLoad<T>(JsonConfigOptions? options, out T? config) where T : JsonConfig, new()
        {
            return TryLoad(new T().DefaultPath, options, out config);
        }

        public static bool TryLoad<T>(string path, JsonConfigOptions? options, out T? config) where T : JsonConfig, new()
        {
            try
            {
                config = Load<T>(path, options);
                return true;
            }
            catch
            {
                config = default;
                return false;
            }
        }

        public void Save()
        {
            Save(Path, Options);
        }

        public void Save(string path)
        {
            Save(path, Options);
        }

        public void Save(JsonConfigOptions? options)
        {
            Save(Path, options);
        }

        public void Save(string path, JsonConfigOptions? options)
        {
            OnBeforeSave();
            var jco = options ?? GlobalOptions;
            string json = JsonSerializer.Serialize(this, GetType(), jco.SerializerOptions);
            File.WriteAllText(path, json);
            OnAfterSave();
        }

        public bool TrySave()
        {
            return TrySave(Path, Options);
        }

        public bool TrySave(string path)
        {
            return TrySave(path, Options);
        }

        public bool TrySave(JsonConfigOptions? options)
        {
            return TrySave(Path, options);
        }

        public bool TrySave(string path, JsonConfigOptions? options)
        {
            try
            {
                Save(path, options);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
