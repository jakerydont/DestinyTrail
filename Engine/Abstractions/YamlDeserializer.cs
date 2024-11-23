using YamlDotNet.Serialization;

namespace DestinyTrail.Engine.Abstractions;
public class YamlDeserializer : IYamlDeserializer
{
    public T Deserialize<T>(string input)
    {
        var deserializer = new DeserializerBuilder()
            .WithTypeResolver(new StaticTypeResolver())
            .Build();
        return deserializer.Deserialize<T>(input);
    }

    public class StaticTypeResolver : ITypeResolver
    {
        public Type Resolve(Type staticType, object? value)
        {
            if (value is IDictionary<object, object> && staticType == typeof(IInventoryItem))
            {
                return typeof(InventoryItem); // Use the concrete class
            }
            return staticType;
        }
    }
}
