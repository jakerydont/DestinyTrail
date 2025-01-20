namespace DestinyTrail.Engine.Abstractions;
public interface IFileReader
{
    string ReadAllText(string path);
    Task<string> ReadAllTextAsync(string path);
}
