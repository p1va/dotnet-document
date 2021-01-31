using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DotnetDocument.Tools.Utils
{
    /// <summary>
    /// The version utils class
    /// </summary>
    internal static class VersionUtils
    {
        /// <summary>
        /// Gets the runtime version
        /// </summary>
        /// <returns>The string</returns>
        internal static string GetRuntimeVersion()
        {
            var pathParts = typeof(string).Assembly.Location.Split('\\', '/');
            var netCoreAppIndex = Array.IndexOf(pathParts, "Microsoft.NETCore.App");

            return pathParts[netCoreAppIndex + 1];
        }

        /// <summary>
        /// Describes whether try get version
        /// </summary>
        /// <param name="version">The version</param>
        /// <returns>The bool</returns>
        internal static bool TryGetVersion([NotNullWhen(true)] out string? version)
        {
            var attribute = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (attribute is not null)
            {
                version = attribute.InformationalVersion;

                return true;
            }

            version = null;

            return false;
        }
    }
}
