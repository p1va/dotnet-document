using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace DotnetDocument.Tools.Workspace
{
    /// <summary>
    /// The workspace factory class
    /// </summary>
    public static class WorkspaceFactory
    {
        /// <summary>
        /// Describes whether try find cs proj
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="csprojFile">The csproj file</param>
        /// <returns>The bool</returns>
        public static bool TryFindCsProj(string path, [NotNullWhen(true)] out string? csprojFile)
        {
            csprojFile = Directory
                .EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            return csprojFile is not null;
        }

        /// <summary>
        /// Describes whether try find sln
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="slnFile">The sln file</param>
        /// <returns>The bool</returns>
        public static bool TryFindSln(string path, [NotNullWhen(true)] out string? slnFile)
        {
            slnFile = Directory
                .EnumerateFiles(path, "*.sln", SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            return slnFile is not null;
        }

        /// <summary>
        /// Describes whether is cs proj
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The bool</returns>
        public static bool IsCsProj(string path) => path.EndsWith(".csproj");

        /// <summary>
        /// Describes whether is sln
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The bool</returns>
        public static bool IsSln(string path) => path.EndsWith(".sln");

        /// <summary>
        /// Gets the default target path
        /// </summary>
        /// <returns>The current folder</returns>
        public static string GetDefaultTargetPath()
        {
            // TODO: Retrieve current directory
            var currentFolder = Directory.GetCurrentDirectory();

            // Is there any sln?
            if (TryFindSln(currentFolder, out var slnFilePath)) return slnFilePath;

            // Is there any sln?
            if (TryFindCsProj(currentFolder, out var csprojFile)) return csprojFile;

            return currentFolder;
        }

        /// <summary>
        /// Creates the path
        /// </summary>
        /// <param name="path">The path</param>
        /// <param name="include">The include</param>
        /// <param name="exclude">The exclude</param>
        /// <exception cref="FileNotFoundException">No file or folder found at path {path} </exception>
        /// <returns>The workspace</returns>
        public static IWorkspace Create(string path, IEnumerable<string> include, IEnumerable<string> exclude)
        {
            var isExistingFile = File.Exists(path);
            var isExistingFolder = Directory.Exists(path);

            if (isExistingFile)
            {
                if (IsSln(path)) return new SolutionWorkspace(path, include, exclude);

                if (IsCsProj(path)) return new ProjectWorkspace(path, include, exclude);

                // Is cs file?
                return new FileWorkspace(path, include, exclude);
            }

            if (isExistingFolder) return new FolderWorkspace(path, include, exclude);

            throw new FileNotFoundException($"No file or folder found at path {path}", path);
        }
    }
}
