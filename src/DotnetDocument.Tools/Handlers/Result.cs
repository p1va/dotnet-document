namespace DotnetDocument.Tools.Handlers
{
    public enum Result : int
    {
        Success = 0,
        GeneralError = 1,
        ArgsParsingError = 2,
        FileNotFound = 3,
        UndocumentedMembers = 4,
    }
}
