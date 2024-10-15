
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DestinyTrail.Engine {
    public static class Utility {
        public static T LoadYaml<T>(string yamlFilePath)
        {
            var yaml = File.ReadAllText(yamlFilePath);
            var deserializer = new DeserializerBuilder()
                .Build();
            return deserializer.Deserialize<T>(yaml); 
        }
    }
}