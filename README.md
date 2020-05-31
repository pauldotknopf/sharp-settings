# SharpSettingsStore

A very simple low-friction approach to persisting settings.

[![SharpSettingsStore](https://img.shields.io/nuget/v/SharpSettingsStore.svg?style=flat-square&label=SharpSettingsStore)](http://www.nuget.org/packages/SharpSettingsStore/)

## Example

Settings definition:

```csharp
[SettingName("my-settings")]
public class MySettings
{
    public MySettings()
    {
        MyString = "default";
        MyInt = 30;
    }
    
    public string MyString { get; set; }
    
    public int MyInt { get; set; }
}
```

Usage:

```csharp
ISettingsService settingsService = /**/;

// Method 1
var settings = await settingsService.GetSetting<MySettings>();
settings.MyInt = 45;
settings.MyString = "updated";
await settingsService.SaveSetting(settings);

// Method 2
await settingsService.SaveSetting<MySettings>(x =>
{
    x.MyInt = 45;
    x.MyString = "updated";
});
```