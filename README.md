[![NuGet](https://img.shields.io/nuget/v/Asjc.JsonConfig)](https://www.nuget.org/packages/Asjc.JsonConfig/)

Asjc.JsonConfig helps you quickly load and save configurations via JSON.

**Use `Asjc.*` instead of `AS.*` and `AsUtils.*`!**

## Quick Start

```csharp
public class Settings : JsonConfig
{
    public string MyString { get; set; }
}
```

```csharp
var settings = JsonConfig.Load<Settings>();
settings.MyString = "Hello!";
settings.Save();
```

## More Examples

### Tiny Log

```csharp
public class Log : JsonConfig
{
    protected override string DefaultPath => $"Logs\\{DateTime.Now:yyyyMMdd}.json";

    protected override JsonConfigOptions DefaultOptions => new() { SaveNew = false };

    public List<string> Messages { get; set; } = new();
}
```

