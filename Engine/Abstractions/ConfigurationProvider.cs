using System;

namespace DestinyTrail.Engine.Abstractions;

public class ConfigurationProvider : IConfigurationProvider
{
    public string? GetAppSetting(string key)
    {
        try
        {
            return ConfigurationManager.AppSettings[key] ?? null;
        }
        catch (ConfigurationErrorsException)
        {
            return null;
        }
    }
}