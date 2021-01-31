using System;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Threading.Tasks;
using DotnetDocument.Tools.Utils;

namespace DotnetDocument.Tools.CLI
{
    internal static class MeasureMiddleware
    {
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
