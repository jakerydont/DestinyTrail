using System;
using System.Configuration;

namespace DestinyTrail.Blazor;

public class ConfigurationProvider : DestinyTrail.Engine.Abstractions.IConfigurationProvider
{
    IConfigurationRoot _config {get;set;}

    public ConfigurationProvider()
    {
        _config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    }
    public string? GetAppSetting(string key)
    {
        try 
        {  
            //GetSection("YourSectionNameFromAppSettings.JSON").Value;
            var val = _config.GetValue(typeof(string), key);
 
            return val?.ToString() ?? throw new Exception($"key {key} not found");
        }
        catch (ConfigurationErrorsException)
        {
            return null;
        }
    }
}