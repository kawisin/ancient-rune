using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Shares.Data
{
    public class InstanceDatabase
    {
        public Header Header { get; set; }
        public List<InstanceConfig> Body { get; set; } = new();
    }

    public class Header
    {
        public string Type { get; set; }
        public int Version { get; set; }
    }

    public class InstanceConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TimeLimit { get; set; }
        public int IdleTimeOut { get; set; }
        public bool Destroyable { get; set; }
        public EnterConfig Enter { get; set; } = new();
    }

    public class EnterConfig
    {
        public string Map { get; set; } = "default_map";
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public static class ConfigLoader
    {
        public static InstanceConfig LoadInstanceConfig(string instanceName)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "InstanceDB.yaml");

            if (!File.Exists(path))
                throw new FileNotFoundException($"YAML config not found at: {path}");

            var yaml = File.ReadAllText(path);

            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();

            var database = deserializer.Deserialize<InstanceDatabase>(yaml);

            var config = database.Body.FirstOrDefault(i => i.Name.Equals(instanceName, StringComparison.OrdinalIgnoreCase));

            if (config != null)
                return config;

            throw new Exception($"Instance config not found for: {instanceName}");
        }
    }
}