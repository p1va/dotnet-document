using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using DotnetDocument.Utils;
using Serilog;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DotnetDocument.Tools.Config
{
    /// <summary>
    /// The yaml class
    /// </summary>
    public static class Yaml
    {
        /// <summary>
        /// Deserializes the file path
        /// </summary>
        /// <typeparam name="TContent">The content</typeparam>
        /// <param name="filePath">The file path</param>
        /// <exception cref="FileNotFoundException">Config file {filePath} doesn't exit</exception>
        /// <returns>The content</returns>
        public static TContent Deserialize<TContent>(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Config file {filePath} doesn't exit");

            var content = File.ReadAllText(filePath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            try
            {
                return deserializer.Deserialize<TContent>(content);
            }
            catch (YamlException e)
            {
                Log.Logger.Error(e.Message);

                if (e.InnerException is SerializationException serializationException)
                    Log.Logger.Error(serializationException.Demystify().ToString());

                if (e.InnerException is YamlException yamlException) Log.Logger.Error(yamlException.Message);

                throw;
            }
        }

        /// <summary>
        /// Serializes the config
        /// </summary>
        /// <typeparam name="TContent">The content</typeparam>
        /// <param name="config">The config</param>
        /// <returns>The string</returns>
        public static string Serialize<TContent>(TContent config)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .DisableAliases()
                .Build();

            return serializer.Serialize(OnlyWhen.NotNull(config, nameof(config)));
        }
    }
}
