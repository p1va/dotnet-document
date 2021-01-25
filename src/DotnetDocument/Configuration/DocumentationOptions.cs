using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Configuration
{
    public static class TemplateKeys
    {
        public const string Name = "{name}";
        public const string Verb = "{verb}";
        public const string Object = "{object}";
        public const string FirstParam = "{param1}";
        public const string Accessors = "{accessors}";
        public const string EnumName = "{enum}";
    }

    public abstract class MemberDocumentationOptionsBase
    {
        public bool Enabled { get; init; } = true;
        public bool Required { get; init; } = true;
        public abstract SyntaxKind GetSyntaxKind();
    }

    public class DefaultMemberDocumentationOptions : MemberDocumentationOptionsBase
    {
        public ExtendedSummaryDocumentationOptions Summary { get; init; } = new($"The {TemplateKeys.Name}");
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.None;
    }

    public class CtorDocumentationOptions : MemberDocumentationOptionsBase
    {
        public SummaryDocumentationOptions Summary { get; init; } =
            new($"Creates a new instance of the {TemplateKeys.Name} class");

        public ParamsDocumentationOptions Parameters { get; set; } = new();
        public ExceptionDocumentationOptions Exceptions { get; set; } = new();
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.ConstructorDeclaration;
    }

    public class ClassDocumentationOptions : MemberDocumentationOptionsBase
    {
        public ExtendedSummaryDocumentationOptions Summary { get; init; } = new($"The {TemplateKeys.Name} class");
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.ClassDeclaration;
    }

    public class InterfaceDocumentationOptions : MemberDocumentationOptionsBase
    {
        public ExtendedSummaryDocumentationOptions Summary { get; init; } =
            new($"The {TemplateKeys.Name} interface");

        public override SyntaxKind GetSyntaxKind() => SyntaxKind.InterfaceDeclaration;
    }

    public class MethodDocumentationOptions : MemberDocumentationOptionsBase
    {
        public MethodSummaryDocumentationOptions Summary { get; init; } = new();
        public ParamsDocumentationOptions Parameters { get; set; } = new();
        public ParamsDocumentationOptions TypeParameters { get; set; } = new();
        public ExceptionDocumentationOptions Exceptions { get; set; } = new();
        public ReturnsDocumentationOptions Returns { get; set; } = new();
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.MethodDeclaration;
    }

    public class PropertyDocumentationOptions : MemberDocumentationOptionsBase
    {
        public SummaryDocumentationOptions Summary { get; init; } =
            new($"{TemplateKeys.Accessors} the value of the {TemplateKeys.Name}");

        public override SyntaxKind GetSyntaxKind() => SyntaxKind.PropertyDeclaration;
    }

    public class EnumDocumentationOptions : MemberDocumentationOptionsBase
    {
        public SummaryDocumentationOptions Summary { get; init; } = new($"The {TemplateKeys.Name} enum");
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.EnumDeclaration;
    }

    public class EnumMemberDocumentationOptions : MemberDocumentationOptionsBase
    {
        public SummaryDocumentationOptions Summary { get; init; } =
            new($"The {TemplateKeys.Name} {TemplateKeys.EnumName}");

        public override SyntaxKind GetSyntaxKind() => SyntaxKind.EnumDeclaration;
    }

    public class SummaryDocumentationOptions
    {
        public SummaryDocumentationOptions()
        {
        }

        public SummaryDocumentationOptions(string template) => (Template) = (template);
        public string Template { get; init; } = $"The {TemplateKeys.Name}";
        public bool NewLine { get; init; } = true;
        public bool IncludeComments { get; init; } = false;
    }

    public class StaticMethodSummaryOptions
    {
        public string BoolMethod { get; init; } = $"describes whether {TemplateKeys.Verb}";
        public string ZeroArgsOneWordMethod { get; init; } = $"{TemplateKeys.Verb}";
    }

    public class InstanceMethodSummaryOptions
    {
        public string BoolMethod { get; init; } = $"describes whether this instance {TemplateKeys.Verb}";

        public string ZeroArgsOneWordMethod { get; init; } = $"{TemplateKeys.Verb} this instance";
    }

    public class MethodSummaryDocumentationOptions
    {
        public MethodSummaryDocumentationOptions()
        {
        }

        public bool NewLine { get; init; } = true;
        public bool IncludeComments { get; init; } = false;

        public StaticMethodSummaryOptions Static { get; init; } = new();
        public InstanceMethodSummaryOptions Instance { get; init; } = new();

        public string TestMethod { get; init; } = $"tests that {TemplateKeys.Verb}";

        public string ManyArgsOneWordMethod { get; init; } = $"{TemplateKeys.Verb} the {TemplateKeys.FirstParam}";

        public string ManyArgsManyWordMethod { get; init; } =
            $"{TemplateKeys.Verb} the {TemplateKeys.Object} using the specified {TemplateKeys.FirstParam}";

        public string Default { get; init; } = $"{TemplateKeys.Verb} the {TemplateKeys.Object}";
    }

    public class ExtendedSummaryDocumentationOptions
    {
        public ExtendedSummaryDocumentationOptions()
        {
        }

        public ExtendedSummaryDocumentationOptions(string template) => (Template) = (template);
        public string Template { get; init; } = $"The {TemplateKeys.Name}";
        public bool NewLine { get; init; } = true;
        public bool IncludeComments { get; init; } = false;
        public bool IncludeInheritance { get; init; } = true;
        public string InheritanceTemplate { get; init; } = $"Inherits from {TemplateKeys.Name}";
    }

    public class ExceptionDocumentationOptions
    {
        public bool Enabled { get; init; } = true;
    };

    public class ParamsDocumentationOptions
    {
        public bool Enabled { get; init; } = true;
        public string Template { get; init; } = $"The {TemplateKeys.Name}";
    };

    public class ReturnsDocumentationOptions
    {
        public bool Enabled { get; init; } = true;
        public string Template { get; init; } = $"The {TemplateKeys.Name}";
    };

    public class DocumentationOptions
    {
        public string? Version { get; set; }
        public ClassDocumentationOptions Class { get; init; } = new();
        public InterfaceDocumentationOptions Interface { get; init; } = new();
        public CtorDocumentationOptions Constructor { get; init; } = new();
        public MethodDocumentationOptions Method { get; init; } = new();
        public PropertyDocumentationOptions Property { get; init; } = new();
        public EnumDocumentationOptions Enum { get; init; } = new();
        public EnumMemberDocumentationOptions EnumMember { get; init; } = new();
        public DefaultMemberDocumentationOptions DefaultMember { get; init; } = new();

        public List<string> PrefixesToRemove { get; init; } = new()
        {
            "_",
        };

        public List<string> SuffixesToRemove { get; init; } = new()
        {
            "Class",
            "Async"
        };

        public List<string> TestAttributes { get; init; } = new()
        {
            "Theory",
            "Fact",
            "TestMethod",
            "Test",
            "TestCase",
            "DataTestMethod"
        };

        public Dictionary<string, string> Verbs { get; init; } = new()
        {
            {
                "to",
                "returns"
            },
            {
                "from",
                "creates"
            },
            {
                "as",
                "converts"
            },
            {
                "with",
                "adds"
            },
            {
                "setup",
                "setup"
            },
            {
                "main",
                "main"
            },
        };

        public Dictionary<string, string> Aliases { get; init; } = new()
        {
            {
                "sut",
                "system under test"
            }
        };

        public List<MemberDocumentationOptionsBase> ToList() => new()
        {
            Class,
            Interface,
            Constructor,
            Method,
            Property,
            Enum,
            EnumMember,
            DefaultMember
        };
    }
}
