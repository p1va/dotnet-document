using System;
using System.Collections.Generic;
using System.IO;

namespace DotnetDocument.Tools.Workspace
{
    public class FolderWorkspace
    {
        private readonly string _mainPath;
        private readonly IList<string> _includePaths;
        private readonly IList<string> _excludePaths;

        public FolderWorkspace(string mainPath, string includePaths, string excludePaths) =>
            (_mainPath, _includePaths, _excludePaths) =
            (mainPath, includePaths?.Split(" "), excludePaths?.Split(" "));

        public IEnumerable<string> LoadFiles()
        {
            var isFile = File.Exists(_mainPath);
            var isFolder = Directory.Exists(_mainPath);

            if (isFile)
            {
                yield return _mainPath;
            }
            else if (isFolder)
            {
                foreach (var filePath in Directory.EnumerateFiles(_mainPath, "*.cs", SearchOption.AllDirectories))
                {
                    yield return filePath;
                }
            }
            else
            {
                throw new ArgumentException($"No file or folder found at path {_mainPath}");
            }
        }
    }
}
