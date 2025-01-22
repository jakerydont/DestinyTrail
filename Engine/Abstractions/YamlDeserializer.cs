using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using YamlDotNet.Serialization;

namespace DestinyTrail.Engine.Abstractions;
public class YamlDeserializer : IYamlDeserializer
{
public T Deserialize<T>(string input)
{
    var deserializer = new DeserializerBuilder()
        .WithTypeResolver(new StaticTypeResolver())
        .Build();
    try
    {
        return deserializer.Deserialize<T>(input) 
            ?? throw new Exception($"Deserialization failed: input: {input}, type: {typeof(T)}");
    }
    catch (Exception ex)
    {
        throw new Exception($"Error deserializing input to type {typeof(T)}: {ex.Message}", ex);
    }
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
