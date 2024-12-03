using System;

namespace DestinyTrail.Engine.Abstractions;

public interface IConfigurationProvider
{
    string? GetAppSetting(string key);
}
