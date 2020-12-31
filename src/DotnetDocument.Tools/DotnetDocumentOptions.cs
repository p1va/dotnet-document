namespace DotnetDocument.Tools
{
    public class DeclarationDocumentationOptions
    {
        public bool Enable { get; set; }
        public SummaryDocumentationOptions Summary { get; set; }
    }

    public class SummaryDocumentationOptions
    {
        public string Template { get; set; }
        public bool NewLine { get; set; } = true;
        public bool Inheritance { get; set; } = true;
    }

    public class DotnetDocumentOptions
    {
        public DeclarationDocumentationOptions Class { get; set; }
        public DeclarationDocumentationOptions Constructor { get; set; }
        public DeclarationDocumentationOptions Method { get; set; }
    }
}
