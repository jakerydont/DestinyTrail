using System;
using System.Text.Json;

namespace DestinyTrail.TwitchIntegration;

public class TwitchAuth
{
    private const string SecretsFilePath = "twitch-secrets.json";  // Path to the secrets file

    public static TwitchSecrets LoadSecrets()
    {
        try
        {
            var jsonString = File.ReadAllText(SecretsFilePath);
            return JsonSerializer.Deserialize<TwitchSecrets>(jsonString);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to load Twitch secrets file.", ex);
        }
    }
}
