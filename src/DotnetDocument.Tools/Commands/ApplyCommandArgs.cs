using System.Collections.Generic;
using CommandLine;

namespace DotnetDocument.Tools.Commands
{
    [Verb("apply", HelpText = "Applies documentation on *.cs files in the folder / solution / project.")]
    public class ApplyCommandArgs
    {
        [Option(
            "dry-run",
            Required = false,
            HelpText =
                "Documents files without saving changes to disk. " +
                "Terminates with a non-zero exit code if any files were documented.")]
        public bool IsDryRun { get; set; }

        [Option(
            'c',
            "config",
            Required = false,
            HelpText = "Set the config file path used to define documentation templates. ")]
        public string ConfigFile { get; set; }

        [Option(
            'v',
            "verbosity",
            Default = "q",
            Required = false,
            HelpText =
                "Set the verbosity level. " +
                "Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]")]
        public string VerbosityLevel { get; set; }

        [Option("include", HelpText =
            "A list of relative file or folder paths to include in documenting. " +
            "All *.cs files are documented if empty.")]
        public string Include { get; set; }

        [Option("exclude", HelpText = "A list of relative file or folder paths to exclude from documenting.")]
        public string Exclude { get; set; }

        [Value(0, MetaName = "path", HelpText =
            "The path to folder / solution / project to operate on. " +
            "If a file is not specified, the command will search the current directory for one.")]
        public string Path { get; set; }
    }
}
