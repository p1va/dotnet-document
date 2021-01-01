namespace DotnetDocument.Configuration
{
    public record DeclarationDocOptions(
        SummaryDocumentationOptions Summary,
        ParametersDocumentationOptions Parameters,
        TypeParametersDocumentationOptions TypeParameters,
        ExceptionsDocumentationOptions Exceptions,
        ReturnsDocumentationOptions Returns,
        bool Enable = true);

    public record SummaryDocumentationOptions(
        string Template = "The {{name}}",
        bool NewLine = true,
        bool IncludeInheritance = true,
        string InheritanceTemplate = "Inherits from {{name}}",
        bool IncludeComments = true);

    public record ExceptionsDocumentationOptions(
        bool Enable = true);

    public record ParametersDocumentationOptions(
        bool Enable = true,
        string Template = "The {{name}}.");

    public record TypeParametersDocumentationOptions(
        bool Enable = true,
        string Template = "The type of the {{name}}.");

    public record ReturnsDocumentationOptions(
        bool Enable = true,
        string Template = "The {{name}}");

    public record DotnetDocumentOptions
    {
        public DeclarationDocOptions Class { get; init; } = new(
            new SummaryDocumentationOptions("The {{name}} class"),
            new ParametersDocumentationOptions(),
            new TypeParametersDocumentationOptions(),
            new ExceptionsDocumentationOptions(),
            new ReturnsDocumentationOptions());

        public DeclarationDocOptions Enum { get; init; } = new(
            new SummaryDocumentationOptions("The {{name}} enum"),
            new ParametersDocumentationOptions(),
            new TypeParametersDocumentationOptions(),
            new ExceptionsDocumentationOptions(),
            new ReturnsDocumentationOptions());

        public DeclarationDocOptions EnumMember { get; init; } = new(
            new SummaryDocumentationOptions("The {{name}} {{enum-name}}"),
            new ParametersDocumentationOptions(),
            new TypeParametersDocumentationOptions(),
            new ExceptionsDocumentationOptions(),
            new ReturnsDocumentationOptions());

        public DeclarationDocOptions Interface { get; init; } = new(
            new SummaryDocumentationOptions("The {{name}} interface"),
            new ParametersDocumentationOptions(),
            new TypeParametersDocumentationOptions(),
            new ExceptionsDocumentationOptions(),
            new ReturnsDocumentationOptions());

        public DeclarationDocOptions Constructor { get; init; } =
            new(new SummaryDocumentationOptions("Creates a new instance of the {{name}} class"),
                new ParametersDocumentationOptions(),
                new TypeParametersDocumentationOptions(),
                new ExceptionsDocumentationOptions(),
                new ReturnsDocumentationOptions());

        public DeclarationDocOptions Method { get; init; } = new(
            new SummaryDocumentationOptions(),
            new ParametersDocumentationOptions(),
            new TypeParametersDocumentationOptions(),
            new ExceptionsDocumentationOptions(),
            new ReturnsDocumentationOptions());

        public DeclarationDocOptions Property { get; init; } = new(
            new SummaryDocumentationOptions("{{accessors}} the value of the {{name}}"),
            new ParametersDocumentationOptions(),
            new TypeParametersDocumentationOptions(),
            new ExceptionsDocumentationOptions(),
            new ReturnsDocumentationOptions());
    }
}
