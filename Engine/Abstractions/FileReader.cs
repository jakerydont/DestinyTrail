using System.IO;
namespace DestinyTrail.Engine.Abstractions;
public class FileReader : IFileReader
{
    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }
}
