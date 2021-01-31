using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DotnetDocument.Configuration;
using DotnetDocument.Format;
using DotnetDocument.Strategies;
using DotnetDocument.Syntax;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotnetDocument.Performance
{
    /// <summary>
    /// The class documentation benchmarks class
    /// </summary>
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class ClassDocumentationBenchmarks
    {
        /// <summary>
        /// The class with inheritance
        /// </summary>
        public const string ClassWithInheritance = @"
    public class UserRepository : RepositoryBase<User>, IUserRepository where User : Entity
    {
        public User Get(string id)
        {
            return new User();
        }
    }";

        /// <summary>
        /// The class declaration syntax
        /// </summary>
        private ClassDeclarationSyntax? _classDeclarationSyntax;

        /// <summary>
        /// The class documentation strategy
        /// </summary>
        private ClassDocumentationStrategy? _classDocumentationStrategy;

        /// <summary>
        /// The
        /// </summary>
        [Params(1000, 10000)]
        public int N;

        /// <summary>
        /// Setup this instance
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            _classDocumentationStrategy = new ClassDocumentationStrategy(
                NullLogger<ClassDocumentationStrategy>.Instance,
                new HumanizeFormatter(new DocumentationOptions()),
                new ClassDocumentationOptions());

            _classDeclarationSyntax = SyntaxUtils.Parse<ClassDeclarationSyntax>(ClassWithInheritance);
        }

        /// <summary>
        /// Applies the comment
        /// </summary>
        [Benchmark]
        public void ApplyComment() => _classDocumentationStrategy!.Apply(_classDeclarationSyntax!);
    }
}
