using System.Collections.Generic;

namespace DotnetDocument.Format
{
    /// <summary>
    /// The formatter interface
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Formats the name using the specified template
        /// </summary>
        /// <param name="template">The template</param>
        /// <param name="names">The names</param>
        /// <returns>The string</returns>
        string FormatName(string template, params (string key, string value)[] names);

        /// <summary>
        /// Formats the inherits using the specified template
        /// </summary>
        /// <param name="template">The template</param>
        /// <param name="key">The key</param>
        /// <param name="names">The names</param>
        /// <returns>The string</returns>
        string FormatInherits(string template, string key, params string[] names);

        /// <summary>
        /// Conjugates the third person singular using the specified verb
        /// </summary>
        /// <param name="verb">The verb</param>
        /// <returns>The string</returns>
        string ConjugateThirdPersonSingular(string verb);

        /// <summary>
        /// Formats the method using the specified method name
        /// </summary>
        /// <param name="methodName">The method name</param>
        /// <param name="returnType">The return type</param>
        /// <param name="modifiers">The modifiers</param>
        /// <param name="parameters">The parameters</param>
        /// <param name="attributes">The attributes</param>
        /// <returns>The string</returns>
        string FormatMethod(string methodName, string returnType, IEnumerable<string> modifiers,
            IEnumerable<string> parameters, IEnumerable<string> attributes);
    }
}
