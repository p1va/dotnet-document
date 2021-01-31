using System;
using System.Diagnostics.CodeAnalysis;

namespace DotnetDocument.Utils
{
    /// <summary>
    /// The only when class
    /// </summary>
    public static class OnlyWhen
    {
        /// <summary>
        /// Nots the null using the specified value
        /// </summary>
        /// <typeparam name="T">The </typeparam>
        /// <param name="value">The value</param>
        /// <param name="name">The name</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns>The value</returns>
        [return: NotNull]
        public static T NotNull<T>(T? value, string? name = null)
        {
            if (value is null) throw new ArgumentNullException(name);

            return value;
        }
    }
}
