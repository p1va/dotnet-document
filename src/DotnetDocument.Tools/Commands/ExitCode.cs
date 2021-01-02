namespace DotnetDocument.Tools.Commands
{
    public enum ExitCode : int
    {
        Success = 0,
        ArgsParsingError = 2,
        FileNotFound = 3,
        UndocumentedMembers = 4,
    }
}
