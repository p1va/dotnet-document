using System;
using System.Diagnostics.CodeAnalysis;
using Serilog;

namespace DotnetDocument.Tools.Config
{
    /// <summary>
    /// The env var class
    /// </summary>
    public static class EnvVar
    {
        /// <summary>
        /// The config file name
        /// </summary>
        private const string ConfigFileName = "DOTNET_DOCUMENT_CONFIG_FILE";

        /// <summary>
        /// Describes whether try get
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="value">The value</param>
        /// <returns>The bool</returns>
        private static bool TryGet(string name, [NotNullWhen(true)] out string value)
        {
            Log.Logger.Debug("Reading content of env var '{Name}'", name);

            // Process is the only supported target in UNIX
            var envVarContent = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);

            if (!string.IsNullOrWhiteSpace(envVarContent))
            {
                Log.Logger.Debug("env var named '{Name}' content is: {Content}", name, envVarContent);

                value = envVarContent;

                return true;
            }

            Log.Logger.Debug("env var named '{Name}' content is empty", name);

            value = string.Empty;

            return false;
        }

        /// <summary>
        /// Describes whether try get config file
        /// </summary>
        /// <param name="configFilePath">The config file path</param>
        /// <returns>The bool</returns>
        public static bool TryGetConfigFile([NotNullWhen(true)] out string configFilePath) =>
            TryGet(ConfigFileName, out configFilePath);
    }
}
