using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace DotnetDocument.Tools.Utils
{
    /// <summary>
    /// The logging utils class
    /// </summary>
    internal static class LoggingUtils
    {
        /// <summary>
        /// Configures the logger using the specified verbosity
        /// </summary>
        /// <param name="verbosity">The verbosity</param>
        /// <returns>The logger</returns>
        internal static ILogger ConfigureLogger(string? verbosity) => new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}",
                theme: SystemConsoleTheme.Literate)
            .MinimumLevel.Is(ParseLogLevel(verbosity))
            .CreateLogger();

        /// <summary>
        /// Parses the log level using the specified verbosity
        /// </summary>
        /// <param name="verbosity">The verbosity</param>
        /// <returns>The log event level</returns>
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
