using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetDocument.Tools.CLI
{
    /// <summary>
    /// The apply command class
    /// </summary>
    internal static class ApplyCommand
    {
        /// <summary>
        /// The apply description
        /// </summary>
        private const string ApplyDescription = "Applies documentation on *.cs files in the [folder/solution/project].";

        /// <summary>
        /// The verbose description
        /// </summary>
        private const string VerboseDescription =
            "(Default: n) Set the verbosity level. Allowed values are q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic]";

        /// <summary>
        /// The path description
        /// </summary>
        private const string PathDescription =
            "The path to a [folder/solution/project] to operate on. If a file is not specified, the command will search the current directory";

        /// <summary>
        /// The dry run description
        /// </summary>
        private const string DryRunDescription =
            "Documents files without saving changes to disk. Terminates with a non-zero exit code if any files were documented.";

        /// <summary>
        /// The config file path description
        /// </summary>
        private const string ConfigFilePathDescription =
            "Set the config file path used to define documentation templates.";

        /// <summary>
        /// Gets the value of the verbosity levels
        /// </summary>
        private static string[] VerbosityLevels => new[]
        {
            "q",
            "quiet",
            "m",
            "minimal",
            "n",
            "normal",
            "d",
            "detailed",
            "diag",
            "diagnostic"
        };

        /// <summary>
        /// Creates the handler
        /// </summary>
        /// <param name="handler">The handler</param>
        /// <returns>The apply command</returns>
        internal static Command Create(Handler handler)
        {
            var applyCommand = new Command("apply", ApplyDescription)
            {
                new Argument<string>
                {
                    Arity = ArgumentArity.ZeroOrOne,
                    Description = PathDescription,
                    Name = "path"
                },
                new Option<string>(new[]
                {
                    "--config",
                    "-c"
                }, ConfigFilePathDescription)
                {
                    IsRequired = false,
                    Argument = new Argument<string>
                    {
                        Arity = ArgumentArity.ExactlyOne
                    }
                },
                new Option<string>(new[]
                {
                    "--verbosity",
                    "-v"
                }, VerboseDescription)
                {
                    IsRequired = false,
                    Argument = new Argument<string?>
                    {
                        Arity = ArgumentArity.ExactlyOne
                    }.FromAmong(VerbosityLevels)
                },
                new Option(new[]
                {
                    "--dry-run"
                }, DryRunDescription)
            };

            applyCommand.Handler = CommandHandler.Create(new Handler(handler));

            return applyCommand;
        }

        /// <summary>
        /// The handler
        /// </summary>
        internal delegate Task<int> Handler(string path, string verbosity, string config,
            bool dryRun, IConsole console, CancellationToken cancellationToken);
    }
}
