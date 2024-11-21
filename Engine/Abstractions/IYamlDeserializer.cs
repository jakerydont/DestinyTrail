namespace DestinyTrail.Engine.Abstractions;
public interface IYamlDeserializer
{
    T Deserialize<T>(string input);
}
