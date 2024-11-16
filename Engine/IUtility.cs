namespace DestinyTrail.Engine
{
    public interface IUtility
    {
        T LoadYaml<T>(string yamlFilePath);
        T NextOrFirst<T>(IEnumerable<T> collection, Func<T, bool> predicate);
        string Abbreviate(double number);
        string GetFormatted(DateTime date);
    }
}
