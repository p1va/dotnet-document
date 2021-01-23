namespace DotnetDocument.Strategies.Abstractions
{
    public interface IServiceResolver<out TService>
    {
        TService? Resolve(string key);
    }
}
