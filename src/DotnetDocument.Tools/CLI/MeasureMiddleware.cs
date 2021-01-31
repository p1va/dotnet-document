using System;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Threading.Tasks;
using DotnetDocument.Tools.Utils;

namespace DotnetDocument.Tools.CLI
{
    /// <summary>
    /// The measure middleware class
    /// </summary>
    internal static class MeasureMiddleware
    {
        /// <summary>
        /// Handles the context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="next">The next</param>
        internal static async Task Handle(InvocationContext context, Func<InvocationContext, Task> next)
        {
            if (context.ParseResult.Directives.Contains("apply"))
            {
                var stopwatch = Stopwatch.StartNew();

                await next(context);

                stopwatch.Stop();

                var logger = LoggingUtils.ConfigureLogger(null);

                logger.Information("Completed in {time} ms", stopwatch.ElapsedMilliseconds);
            }
            else
            {
                await next(context);
            }
        }
    }
}
