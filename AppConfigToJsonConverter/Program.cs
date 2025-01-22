using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: AppConfigToJsonConverter <sourceConfigPath> <destJsonPath>");
            return;
        }

        string sourceConfigPath = args[0];
        string destJsonPath = args[1];

        try
        {
            var xml = XDocument.Load(sourceConfigPath);
            var appSettings = new Dictionary<string, string>();

            foreach (var addElement in xml.Descendants("appSettings").Descendants("add"))
            {
                string key = addElement.Attribute("key")?.Value ?? throw new Exception();
                string value = addElement.Attribute("value")?.Value ?? throw new Exception();

                if (key != null && value != null)
                {
                    appSettings[key] = value;
                }
            }

            // Handle duplicate keys
            var uniqueSettings = new Dictionary<string, string>();
            foreach (var setting in appSettings)
            {
                if (!uniqueSettings.ContainsKey(setting.Key))
                {
                    uniqueSettings.Add(setting.Key, setting.Value);
                }
            }

            string json = JsonConvert.SerializeObject(uniqueSettings, Formatting.Indented);
            File.WriteAllText(destJsonPath, json);

            Console.WriteLine($"Converted {sourceConfigPath} to {destJsonPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
