using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DotnetDocument.Performance
{
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class ClassDocumentationBenchmarks
    {
        public const string ClassWithInheritance = @"
    public class UserRepository : RepositoryBase<User>, IUserRepository where User : Entity
    {
        public User Get(string id)
        {
            return new User();
        }
    }";

        private ClassDocumentationStrategy? _classDocumentationStrategy;
        private ClassDeclarationSyntax? _classDeclarationSyntax;

        [Params(1000, 10000)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            _classDocumentationStrategy = new ClassDocumentationStrategy(
                NullLogger<ClassDocumentationStrategy>.Instance, new HumanizeFormatter(),
                new ClassDocumentationOptions());

            _classDeclarationSyntax = SyntaxUtils.Parse<ClassDeclarationSyntax>(ClassWithInheritance);
        }

        [Benchmark]
        public void ApplyComment() => _classDocumentationStrategy!.Apply(_classDeclarationSyntax!);
    }
}
