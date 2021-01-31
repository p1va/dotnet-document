using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;

namespace DotnetDocument.Configuration
{
    /// <summary>
    /// The member documentation options base class
    /// </summary>
    public abstract class MemberDocumentationOptionsBase
    {
        /// <summary>
        /// Gets or inits the value of the enabled
        /// </summary>
        public bool Enabled { get; init; } = true;

        /// <summary>
        /// Gets or inits the value of the required
        /// </summary>
        public bool Required { get; init; } = true;

        /// <summary>
        /// Gets the syntax kind
        /// </summary>
        /// <returns>The syntax kind</returns>
        public abstract SyntaxKind GetSyntaxKind();
    }

    /// <summary>
    /// The default member documentation options class
    /// </summary>
    /// <seealso cref="MemberDocumentationOptionsBase" />
    public class DefaultMemberDocumentationOptions : MemberDocumentationOptionsBase
    {
        /// <summary>
        /// Gets or inits the value of the summary
        /// </summary>
        public ExtendedSummaryDocumentationOptions Summary { get; init; } = new($"The {TemplateKeys.Name}");

        /// <summary>
        /// Gets the syntax kind
        /// </summary>
        /// <returns>The syntax kind</returns>
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.None;
    }

    /// <summary>
    /// The ctor documentation options class
    /// </summary>
    /// <seealso cref="MemberDocumentationOptionsBase" />
    public class CtorDocumentationOptions : MemberDocumentationOptionsBase
    {
        /// <summary>
        /// Gets or inits the value of the summary
        /// </summary>
        public SummaryDocumentationOptions Summary { get; init; } =
            new($"Initializes a new instance of the {TemplateKeys.Name} class");

        /// <summary>
        /// Gets or sets the value of the parameters
        /// </summary>
        public ParamsDocumentationOptions Parameters { get; set; } = new();

        /// <summary>
        /// Gets or sets the value of the exceptions
        /// </summary>
        public ExceptionDocumentationOptions Exceptions { get; set; } = new();

        /// <summary>
        /// Gets the syntax kind
        /// </summary>
        /// <returns>The syntax kind</returns>
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.ConstructorDeclaration;
    }

    /// <summary>
    /// The class documentation options class
    /// </summary>
    /// <seealso cref="MemberDocumentationOptionsBase" />
    public class ClassDocumentationOptions : MemberDocumentationOptionsBase
    {
        /// <summary>
        /// Gets or inits the value of the summary
        /// </summary>
        public ExtendedSummaryDocumentationOptions Summary { get; init; } = new($"The {TemplateKeys.Name} class");

        /// <summary>
        /// Gets the syntax kind
        /// </summary>
        /// <returns>The syntax kind</returns>
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.ClassDeclaration;
    }

    /// <summary>
    /// The interface documentation options class
    /// </summary>
    /// <seealso cref="MemberDocumentationOptionsBase" />
    public class InterfaceDocumentationOptions : MemberDocumentationOptionsBase
    {
        /// <summary>
        /// Gets or inits the value of the summary
        /// </summary>
        public ExtendedSummaryDocumentationOptions Summary { get; init; } =
            new($"The {TemplateKeys.Name} interface");

        /// <summary>
        /// Gets the syntax kind
        /// </summary>
        /// <returns>The syntax kind</returns>
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.InterfaceDeclaration;
    }

    /// <summary>
    /// The method documentation options class
    /// </summary>
    /// <seealso cref="MemberDocumentationOptionsBase" />
    public class MethodDocumentationOptions : MemberDocumentationOptionsBase
    {
        /// <summary>
        /// Gets or inits the value of the summary
        /// </summary>
        public MethodSummaryDocumentationOptions Summary { get; init; } = new();

        /// <summary>
        /// Gets or sets the value of the parameters
        /// </summary>
        public ParamsDocumentationOptions Parameters { get; set; } = new();

        /// <summary>
        /// Gets or sets the value of the type parameters
        /// </summary>
        public ParamsDocumentationOptions TypeParameters { get; set; } = new();

        /// <summary>
        /// Gets or sets the value of the exceptions
        /// </summary>
        public ExceptionDocumentationOptions Exceptions { get; set; } = new();

        /// <summary>
        /// Gets or sets the value of the returns
        /// </summary>
        public ReturnsDocumentationOptions Returns { get; set; } = new();

        /// <summary>
        /// Gets the syntax kind
        /// </summary>
        /// <returns>The syntax kind</returns>
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.MethodDeclaration;
    }

    /// <summary>
    /// The property documentation options class
    /// </summary>
    /// <seealso cref="MemberDocumentationOptionsBase" />
    public class PropertyDocumentationOptions : MemberDocumentationOptionsBase
    {
        /// <summary>
        /// Gets or inits the value of the summary
        /// </summary>
        public SummaryDocumentationOptions Summary { get; init; } =
            new($"{TemplateKeys.Accessors} the value of the {TemplateKeys.Name}");

        /// <summary>
        /// Gets the syntax kind
        /// </summary>
        /// <returns>The syntax kind</returns>
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.PropertyDeclaration;
    }

    /// <summary>
    /// The enum documentation options class
    /// </summary>
    /// <seealso cref="MemberDocumentationOptionsBase" />
    public class EnumDocumentationOptions : MemberDocumentationOptionsBase
    {
        /// <summary>
        /// Gets or inits the value of the summary
        /// </summary>
        public SummaryDocumentationOptions Summary { get; init; } = new($"The {TemplateKeys.Name} enum");

        /// <summary>
        /// Gets the syntax kind
        /// </summary>
        /// <returns>The syntax kind</returns>
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.EnumDeclaration;
    }

    /// <summary>
    /// The enum member documentation options class
    /// </summary>
    /// <seealso cref="MemberDocumentationOptionsBase" />
    public class EnumMemberDocumentationOptions : MemberDocumentationOptionsBase
    {
        /// <summary>
        /// Gets or inits the value of the summary
        /// </summary>
        public SummaryDocumentationOptions Summary { get; init; } =
            new($"The {TemplateKeys.Name} {TemplateKeys.EnumName}");

        /// <summary>
        /// Gets the syntax kind
        /// </summary>
        /// <returns>The syntax kind</returns>
        public override SyntaxKind GetSyntaxKind() => SyntaxKind.EnumDeclaration;
    }

    /// <summary>
    /// The summary documentation options class
    /// </summary>
    public class SummaryDocumentationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryDocumentationOptions" /> class
        /// </summary>
        public SummaryDocumentationOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SummaryDocumentationOptions" /> class
        /// </summary>
        /// <param name="template">The template</param>
        public SummaryDocumentationOptions(string template) => Template = template;

        /// <summary>
        /// Gets or inits the value of the template
        /// </summary>
        public string Template { get; init; } = $"The {TemplateKeys.Name}";

        /// <summary>
        /// Gets or inits the value of the new line
        /// </summary>
        public bool NewLine { get; init; } = true;

        /// <summary>
        /// Gets or inits the value of the include comments
        /// </summary>
        public bool IncludeComments { get; init; } = false;
    }

    /// <summary>
    /// The static method summary options class
    /// </summary>
    public class StaticMethodSummaryOptions
    {
        /// <summary>
        /// Gets or inits the value of the bool method
        /// </summary>
        public string BoolMethod { get; init; } = $"describes whether {TemplateKeys.Verb}";

        /// <summary>
        /// Gets or inits the value of the zero args one word method
        /// </summary>
        public string ZeroArgsOneWordMethod { get; init; } = $"{TemplateKeys.Verb}";
    }

    /// <summary>
    /// The instance method summary options class
    /// </summary>
    public class InstanceMethodSummaryOptions
    {
        /// <summary>
        /// Gets or inits the value of the bool method
        /// </summary>
        public string BoolMethod { get; init; } = $"describes whether this instance {TemplateKeys.Verb}";

        /// <summary>
        /// Gets or inits the value of the zero args one word method
        /// </summary>
        public string ZeroArgsOneWordMethod { get; init; } = $"{TemplateKeys.Verb} this instance";
    }

    /// <summary>
    /// The method summary documentation options class
    /// </summary>
    public class MethodSummaryDocumentationOptions
    {
        /// <summary>
        /// Gets or inits the value of the new line
        /// </summary>
        public bool NewLine { get; init; } = true;

        /// <summary>
        /// Gets or inits the value of the include comments
        /// </summary>
        public bool IncludeComments { get; init; } = false;

        /// <summary>
        /// Gets or inits the value of the static
        /// </summary>
        public StaticMethodSummaryOptions Static { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the instance
        /// </summary>
        public InstanceMethodSummaryOptions Instance { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the test method
        /// </summary>
        public string TestMethod { get; init; } = $"tests that {TemplateKeys.Verb}";

        /// <summary>
        /// Gets or inits the value of the many args one word method
        /// </summary>
        public string ManyArgsOneWordMethod { get; init; } = $"{TemplateKeys.Verb} the {TemplateKeys.FirstParam}";

        /// <summary>
        /// Gets or inits the value of the many args many word method
        /// </summary>
        public string ManyArgsManyWordMethod { get; init; } =
            $"{TemplateKeys.Verb} the {TemplateKeys.Object} using the specified {TemplateKeys.FirstParam}";

        /// <summary>
        /// Gets or inits the value of the default
        /// </summary>
        public string Default { get; init; } = $"{TemplateKeys.Verb} the {TemplateKeys.Object}";
    }

    /// <summary>
    /// The extended summary documentation options class
    /// </summary>
    public class ExtendedSummaryDocumentationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSummaryDocumentationOptions" /> class
        /// </summary>
        public ExtendedSummaryDocumentationOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedSummaryDocumentationOptions" /> class
        /// </summary>
        /// <param name="template">The template</param>
        public ExtendedSummaryDocumentationOptions(string template) => Template = template;

        /// <summary>
        /// Gets or inits the value of the template
        /// </summary>
        public string Template { get; init; } = $"The {TemplateKeys.Name}";

        /// <summary>
        /// Gets or inits the value of the new line
        /// </summary>
        public bool NewLine { get; init; } = true;

        /// <summary>
        /// Gets or inits the value of the include comments
        /// </summary>
        public bool IncludeComments { get; init; } = false;

        /// <summary>
        /// Gets or inits the value of the include inheritance
        /// </summary>
        public bool IncludeInheritance { get; init; } = true;
    }

    /// <summary>
    /// The exception documentation options class
    /// </summary>
    public class ExceptionDocumentationOptions
    {
        /// <summary>
        /// Gets or inits the value of the enabled
        /// </summary>
        public bool Enabled { get; init; } = true;
    }

    /// <summary>
    /// The params documentation options class
    /// </summary>
    public class ParamsDocumentationOptions
    {
        /// <summary>
        /// Gets or inits the value of the enabled
        /// </summary>
        public bool Enabled { get; init; } = true;

        /// <summary>
        /// Gets or inits the value of the template
        /// </summary>
        public string Template { get; init; } = $"The {TemplateKeys.Name}";
    }

    /// <summary>
    /// The returns documentation options class
    /// </summary>
    public class ReturnsDocumentationOptions
    {
        /// <summary>
        /// Gets or inits the value of the enabled
        /// </summary>
        public bool Enabled { get; init; } = true;

        /// <summary>
        /// Gets or inits the value of the template
        /// </summary>
        public string Template { get; init; } = $"The {TemplateKeys.Name}";
    }

    /// <summary>
    /// The documentation options class
    /// </summary>
    public class DocumentationOptions
    {
        /// <summary>
        /// Gets or sets the value of the version
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Gets or inits the value of the class
        /// </summary>
        public ClassDocumentationOptions Class { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the interface
        /// </summary>
        public InterfaceDocumentationOptions Interface { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the constructor
        /// </summary>
        public CtorDocumentationOptions Constructor { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the method
        /// </summary>
        public MethodDocumentationOptions Method { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the property
        /// </summary>
        public PropertyDocumentationOptions Property { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the enum
        /// </summary>
        public EnumDocumentationOptions Enum { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the enum member
        /// </summary>
        public EnumMemberDocumentationOptions EnumMember { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the default member
        /// </summary>
        public DefaultMemberDocumentationOptions DefaultMember { get; init; } = new();

        /// <summary>
        /// Gets or inits the value of the prefixes to remove
        /// </summary>
        public List<string> PrefixesToRemove { get; init; } = new()
        {
            "_"
        };

        /// <summary>
        /// Gets or inits the value of the suffixes to remove
        /// </summary>
        public List<string> SuffixesToRemove { get; init; } = new()
        {
            "Class",
            "Async"
        };

        /// <summary>
        /// Gets or inits the value of the test attributes
        /// </summary>
        public List<string> TestAttributes { get; init; } = new()
        {
            "Theory",
            "Fact",
            "TestMethod",
            "Test",
            "TestCase",
            "DataTestMethod"
        };

        /// <summary>
        /// Gets or inits the value of the verbs
        /// </summary>
        public Dictionary<string, string> Verbs { get; init; } = new()
        {
            {
                "to", "returns"
            },
            {
                "from", "creates"
            },
            {
                "as", "converts"
            },
            {
                "with", "adds"
            },
            {
                "setup", "setup"
            },
            {
                "main", "main"
            }
        };

        /// <summary>
        /// Gets or inits the value of the aliases
        /// </summary>
        public Dictionary<string, string> Aliases { get; init; } = new()
        {
            {
                "sut", "system under test"
            }
        };

        /// <summary>
        /// Returns the list
        /// </summary>
        /// <returns>A list of member documentation options base</returns>
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
