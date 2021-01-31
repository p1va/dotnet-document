using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace DotnetDocument.Tools.Utils
{
    internal static class VersionUtils
    {
        internal static string GetRuntimeVersion()
        {
            var pathParts = typeof(string).Assembly.Location.Split('\\', '/');
            var netCoreAppIndex = Array.IndexOf(pathParts, "Microsoft.NETCore.App");

            return pathParts[netCoreAppIndex + 1];
        }

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
