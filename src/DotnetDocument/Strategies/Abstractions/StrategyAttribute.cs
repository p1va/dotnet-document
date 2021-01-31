using System;

namespace DotnetDocument.Strategies.Abstractions
{
    /// <summary>
    /// The strategy attribute class
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class StrategyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrategyAttribute" /> class
        /// </summary>
        /// <param name="key">The key</param>
        public StrategyAttribute(string key) => Key = key;

        /// <summary>
        /// Gets the value of the key
        /// </summary>
        public string Key { get; }
    }
}
