namespace DotnetDocument.Tools.Commands
{
    public interface ICommand
    {
        ExitCode Run(CommandArgs args);
    }
}
