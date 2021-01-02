namespace DotnetDocument.Tools.Commands
{
    public interface ICommand<TArgs>
    {
        ExitCode Run(TArgs args);
    }
}
