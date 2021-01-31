namespace DotnetDocument.Tools.Handlers
{
    public interface IDocumentConfigHandler
    {
        Result PrintCurrentConfig();
        Result PrintDefaultConfig();
    }
}
