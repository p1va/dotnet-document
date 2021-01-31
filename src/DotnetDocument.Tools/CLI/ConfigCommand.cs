using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;

namespace DotnetDocument.Tools.CLI
{
    internal static class ConfigCommand
    {
        private const string ConfigDescription = "Prints the tool configuration file.";

        private const string DefaultDescription = "Prints the default documentation config";

        private const string ConfigFilePathDescription = "Set the config file path used to define documentation templates.";

        internal delegate Task<int> Handler(bool @default, string config, IConsole console,
            CancellationToken cancellationToken);

        internal static Command Create(Handler handler)
        {
            var configCommand = new Command("config", ConfigDescription)
            {
                new Option<string>(new[]
                {
                    "--config",
                    "-c"
                }, ConfigFilePathDescription)
                {
                    IsRequired = false,
                    Argument = new Argument<string>()
                    {
                        Arity = ArgumentArity.ExactlyOne,
                    }
                },
                new Option(new[]
                {
                    "--default"
                }, DefaultDescription),
            };

            configCommand.Handler = CommandHandler.Create(new Handler(handler));

            return configCommand;
        }
    }
}
