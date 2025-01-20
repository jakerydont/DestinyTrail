using System.Configuration;
using System.Threading.Tasks;
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
        IConfigurationProvider ConfigurationProvider { get; }

        public Utility() : this ( new YamlDeserializer(), new FileReader(), new ConfigurationProvider() ) {} 

        public Utility(IConfigurationProvider configurationProvider) : this ( new YamlDeserializer(), new FileReader(), configurationProvider) {}
        public Utility(IYamlDeserializer yamlDeserializer, IFileReader fileReader, IConfigurationProvider configurationProvider)
        {
            Deserializer = yamlDeserializer;
            FileReader = fileReader;
            ConfigurationProvider = configurationProvider;
        }

        public T LoadYaml<T>(string yamlFilePath)
        {
            var yaml = FileReader.ReadAllText(yamlFilePath);
            return Deserializer.Deserialize<T>(yaml);
        }

        public async Task<T> LoadYamlAsync<T>(string yamlFilePath)
        {
            var yaml = await FileReader.ReadAllTextAsync(yamlFilePath);
            return Deserializer.Deserialize<T>(yaml);
        }


        public T NextOrFirst<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            var next = collection
                .SkipWhile(item => !predicate(item))
                .Skip(1)
                .FirstOrDefault();

            return next == null || next.Equals(default(T)) ? collection.First() : next;
        }

        public string Abbreviate(double number)
        {
            return Convert.ToInt32(number).ToString();
        }

        public string GetFormatted(DateTime date)
        {
            return $"{date:MMMM d, yyyy}";
        }

        public string GetAppSetting(string settingName)
        {
            return ConfigurationProvider.GetAppSetting(settingName) ?? throw new ConfigurationErrorsException($"{settingName} is not configured.");
        }
    }
}
