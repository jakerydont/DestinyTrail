
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

        public static T NextOrFirst<T>(this IEnumerable<T> collection, Func<T, bool> predicate) => collection
                .SkipWhile(item => !predicate(item))  
                .Skip(1)                               
                .FirstOrDefault() ?? collection.First();

        public static string Abbreviate(this double number) => Convert.ToInt32(number).ToString();
        public static string GetFormatted(this DateTime date) => $"{date:MMMM d, yyyy}";
        
    }
}