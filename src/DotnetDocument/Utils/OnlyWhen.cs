using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DotnetDocument.Utils
{
    public static class OnlyWhen
    {
        [return: NotNull]
        public static T NotNull<T>(T? value, string? name = null)
        {
            if (value is null)
            {
                throw new ArgumentNullException(name);
            }

            return value;
        }
    }
}
