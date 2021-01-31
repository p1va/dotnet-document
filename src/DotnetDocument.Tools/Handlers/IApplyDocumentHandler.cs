namespace DotnetDocument.Tools.Handlers
{
    public interface IApplyDocumentHandler
    {
        Result Apply(string? path, bool isDryRun);
    }
}
