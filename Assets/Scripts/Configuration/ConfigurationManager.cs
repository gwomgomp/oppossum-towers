using System.ComponentModel;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class ConfigurationManager {
    private readonly Dictionary<string, Dictionary<string, string>> configs = new();

    [RuntimeInitializeOnLoadMethod]
    static void LoadConfigurations() {
        var manager = new ConfigurationManager();
        var configurationFiles = Resources.LoadAll<TextAsset>("Configurations");
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        foreach (var file in configurationFiles) {
            var result = deserializer.Deserialize<ConfigurationFile>(file.text);
            manager.AddConfiguration(file.name, result);
        }
        ManagerProvider.Instance.RegisterManager(manager);
    }

    private void AddConfiguration(string name, ConfigurationFile result) {
        Dictionary<string, string> values = new();
        foreach (var config in result.Configurations) {
            values.Add(config.Name, config.Value);
        }
        configs.Add(name, values);
    }

    public T GetConfig<T>(string configGroup, string configId) {
        var value = configs[configGroup][configId];
        return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
    }
}

struct ConfigurationFile {
    public Configuration[] Configurations { get; private set; }
}

struct Configuration {
    public string Name { get; private set; }
    public string Value { get; private set; }
}
