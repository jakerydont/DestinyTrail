using YamlDotNet.Serialization;

namespace DestinyTrail.Engine.Abstractions;
public class YamlDeserializer : IYamlDeserializer
{
    public T Deserialize<T>(string input)
    {
        var deserializer = new DeserializerBuilder().Build();
        return deserializer.Deserialize<T>(input);
    }
}
