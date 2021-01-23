using System.IO;
using System.Runtime.Serialization;
using DotnetDocument.Utils;
using Serilog;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotnetDocument.Tools
{
    public class Yaml
    {
        public static TContent Deserialize<TContent>(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Config file {filePath} doesn't exit");
            }

            var content = File.ReadAllText(filePath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            try
            {
                return deserializer.Deserialize<TContent>(content);
            }
            catch (YamlException e)
            {
                Log.Logger.Error(e.Message);

                if (e.InnerException is SerializationException serializationException)
                {
                    Log.Logger.Error(serializationException.Message);
                }

                if (e.InnerException is YamlException yamlException)
                {
                    Log.Logger.Error(yamlException.Message);
                }

                throw;
            }
        }

        public static string Serialize<TContent>(TContent config)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .DisableAliases()
                .Build();

            return serializer.Serialize(OnlyWhen.NotNull(config, nameof(config)));
        }
    }
}
