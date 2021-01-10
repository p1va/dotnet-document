using System.Collections.Generic;
using CommandLine;

namespace DotnetDocument.Tools.Commands
{
    [Verb("config", HelpText = "Prints the current documentation config.")]
    public class ConfigCommandArgs
    {
        [Option("default", Required = false, HelpText = "Prints the default documentation config.")]
        public bool IsDefaultConfig { get; set; }

        [Option(
            'c',
            "config",
            Required = false,
            HelpText = "Set the config file path used to define documentation templates. ")]
        public string ConfigFile { get; set; }
    }
}
