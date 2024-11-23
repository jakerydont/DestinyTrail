using DestinyTrail.Engine.Abstractions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DestinyTrail.Engine
{
    public class Utility : IUtility
    {
        IYamlDeserializer Deserializer { get; set; }
        IFileReader FileReader { get; set; }
        public Utility() : this (new YamlDeserializer(), new FileReader()) {} 
        public Utility(IYamlDeserializer yamlDeserializer, IFileReader fileReader)
        {
            Deserializer = yamlDeserializer;
            FileReader = fileReader;
        }

        public T LoadYaml<T>(string yamlFilePath)
        {
            var yaml = FileReader.ReadAllText(yamlFilePath);
            return Deserializer.Deserialize<T>(yaml);
        }

        public T NextOrFirst<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            return collection
                .SkipWhile(item => !predicate(item))  
                .Skip(1)                               
                .FirstOrDefault() ?? collection.First();
        }

        public string Abbreviate(double number)
        {
            return Convert.ToInt32(number).ToString();
        }

        public string GetFormatted(DateTime date)
        {
            return $"{date:MMMM d, yyyy}";
        }
    }
}
