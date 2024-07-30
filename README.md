# JsonConfig

[![NuGet](https://img.shields.io/nuget/v/Asjc.JsonConfig)](https://www.nuget.org/packages/Asjc.JsonConfig/)

Quickly load and save configurations via JSON.

## Quick Start

```csharp
public class Settings : JsonConfig
{
    public string? Text { get; set; }
}
```

```csharp
var settings = JsonConfig.Load<Settings>();
if (settings != null)
{
    settings.Text = "Hello!";
    settings.Save();
}
```

## Credits

- Icon from https://www.iconfinder.com/icons/9040309