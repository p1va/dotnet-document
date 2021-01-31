using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace DotnetDocument.Tools.Utils
{
    internal static class LoggingUtils
    {
        internal static ILogger ConfigureLogger(string? verbosity) => new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}",
                theme: SystemConsoleTheme.Literate)
            .MinimumLevel.Is(ParseLogLevel(verbosity))
            .CreateLogger();

        internal static LogEventLevel ParseLogLevel(string? verbosity) => verbosity switch
        {
            "q" => LogEventLevel.Error,
            "quiet" => LogEventLevel.Error,
            "m" => LogEventLevel.Warning,
            "minimal" => LogEventLevel.Warning,
            "n" => LogEventLevel.Information,
            "normal" => LogEventLevel.Information,
            "d" => LogEventLevel.Debug,
            "detailed" => LogEventLevel.Debug,
            "diag" => LogEventLevel.Verbose,
            "diagnostic" => LogEventLevel.Verbose,
            _ => LogEventLevel.Information
        };
    }
}
