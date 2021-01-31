namespace DotnetDocument.Tools.Handlers
{
    /// <summary>
    /// The result enum
    /// </summary>
    public enum Result
    {
        /// <summary>
        /// The success result
        /// </summary>
        Success = 0,

        /// <summary>
        /// The general error result
        /// </summary>
        GeneralError = 1,

        /// <summary>
        /// The args parsing error result
        /// </summary>
        ArgsParsingError = 2,

        /// <summary>
        /// The file not found result
        /// </summary>
        FileNotFound = 3,

        /// <summary>
        /// The undocumented members result
        /// </summary>
        UndocumentedMembers = 4
    }
}
