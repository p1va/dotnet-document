using System.Collections.Generic;

namespace DotnetDocument.Format
{
    public interface IFormatter
    {
        string FormatName(string template, params (string key, string value)[] names);
        string FormatInherits(string template, string key, params string[] names);
        string FormatVerb(string verb);
        string FormatMethod(string name, string returnType, IEnumerable<string> modifiers,
            IEnumerable<string> parameters, IEnumerable<string> attributes);
    }
}
