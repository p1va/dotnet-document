using System.Collections.Generic;

namespace DotnetDocument.Format
{
    public interface IFormatter
    {
        string FormatName(string template, params (string key, string value)[] names);
        string FormatInherits(string template, string key, params string[] names);
        string ConjugateThirdPersonSingular(string verb);
        string FormatMethod(string methodName, string returnType, IEnumerable<string> modifiers,
            IEnumerable<string> parameters, IEnumerable<string> attributes);
    }
}
