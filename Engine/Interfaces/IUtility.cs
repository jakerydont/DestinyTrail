namespace DestinyTrail.Engine.Interfaces;

public interface IUtility
{
    Task<T> LoadYamlAsync<T>(string yamlFilePath);
    T NextOrFirst<T>(IEnumerable<T> collection, Func<T, bool> predicate);
    string Abbreviate(double number);
    string GetFormatted(DateTime date);
    string GetAppSetting(string settingName);
}
