using System;
using System.CommandLine.Invocation;
using System.IO;
using DotnetDocument.Tools.Handlers;
using DotnetDocument.Tools.Utils;

namespace DotnetDocument.Tools.CLI
{
    internal static class ExceptionFilter
    {
        internal static void Handle(Exception exception, InvocationContext context)
        {
            //context.Console.Error.Write(exception.ToStringDemystified());
            var logger = LoggingUtils.ConfigureLogger(null);

            var code = Result.GeneralError;

            // TODO: move to dictionary

            switch (exception)
            {
                case FileNotFoundException fileNotFoundException:

                    logger.Error(fileNotFoundException.Message);
                    code = Result.FileNotFound;

                    break;

                case Exception ex:
                    code = Result.GeneralError;
                    logger.Error("An error occurred: {error}", ex.Message);

                    break;
            }

            context.ResultCode = (int)code;
        }
    }
}
