using System;

namespace DotnetDocument.Strategies.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class StrategyAttribute : Attribute
    {
        public StrategyAttribute(string key) => (Key) = (key);
        public string Key { get; }
    }
}
