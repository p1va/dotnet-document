using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Syntax
{
    public static class DocumentationFactory
    {
        public static SyntaxToken XmlNewLineToken(SyntaxTrivia indentationTrivia)
        {
            return SyntaxFactory
                .XmlTextNewLine("\n", false)
                .WithTrailingTrivia(SyntaxFactory.TriviaList(
                    indentationTrivia, SyntaxFactory
                        .DocumentationCommentExterior("/// ")));
        }

        /*
         * Creates a new instance of the {{System.string}} class.
         * Inherits from {{System.object}}.
         */
        public static XmlElementSyntax Summary(IEnumerable<string> summaryLines, SyntaxToken xmlIndentedNewLine,
            bool keepSameLine)
        {
            var xmlSummaryLines = new List<XmlNodeSyntax>();

            xmlSummaryLines.Add(SyntaxFactory
                .XmlText(SyntaxFactory.TokenList(
                    xmlIndentedNewLine
                )));

            foreach (var summaryLine in summaryLines)
            {
                // if (summaryLine.Contains("{{") || summaryLine.Contains("}}"))
                // {
                //     // TODO: Handle see element
                // }
                // else
                // {
                xmlSummaryLines.Add(SyntaxFactory
                    .XmlText(SyntaxFactory.TokenList(
                        SyntaxFactory.XmlTextLiteral(summaryLine)
                    )));

                xmlSummaryLines.Add(SyntaxFactory
                    .XmlText(SyntaxFactory.TokenList(
                        xmlIndentedNewLine
                    )));
                //}
            }

            // Declare the summary XML element
            return SyntaxFactory.XmlSummaryElement(xmlSummaryLines.ToArray());
        }

        public static XmlElementSyntax Returns(string description)
        {
            return SyntaxFactory
                .XmlReturnsElement(SyntaxFactory
                    .XmlText(description));
        }

        public static XmlElementSyntax Exception(string exception, string description)
        {
            // Declare the returns XML element
            return SyntaxFactory.XmlExceptionElement(SyntaxFactory
                    .TypeCref(SyntaxFactory.ParseTypeName(exception)),
                SyntaxFactory.XmlText(description));
        }

        public static XmlEmptyElementSyntax See(string type)
        {
            return SyntaxFactory
                .XmlSeeElement(SyntaxFactory
                    .TypeCref(SyntaxFactory
                        .ParseTypeName(type)));
        }

        public static XmlElementSyntax Param(string name, string description)
        {
            return SyntaxFactory
                .XmlParamElement(name, SyntaxFactory
                    .XmlText(description));
        }

        public static XmlElementSyntax TypeParam(string type, string description)
        {
            // Declare type params
            var attributeList = SyntaxFactory.List<XmlAttributeSyntax>().Add(SyntaxFactory
                .XmlNameAttribute(
                    SyntaxFactory.XmlName(" name"),
                    SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                    SyntaxFactory.IdentifierName(type),
                    SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken)));

            var startTag = SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("typeparam"))
                .WithAttributes(attributeList);

            var descriptionXml = new SyntaxList<XmlNodeSyntax>(SyntaxFactory.XmlText(description));

            var endTag = SyntaxFactory.XmlElementEndTag(SyntaxFactory
                .XmlName("typeparam"));

            // Declare type param
            return SyntaxFactory
                .XmlElement(startTag, descriptionXml, endTag);
        }

        public static DocumentationCommentTriviaSyntax XmlDocument(
            XmlTextSyntax xmlIndentedNewLine,
            XmlElementSyntax summary,
            List<XmlElementSyntax> typeParameters = null,
            List<XmlElementSyntax> parameters = null,
            List<XmlElementSyntax> exceptions = null,
            XmlElementSyntax returns = null)
        {
            var list = new List<XmlNodeSyntax>()
            {
                summary
            };

            if (typeParameters is not null)
            {
                foreach (var typeParameter in typeParameters)
                {
                    list.Add(xmlIndentedNewLine);
                    list.Add(typeParameter);
                }
            }

            if (parameters is not null)
            {
                foreach (var parameter in parameters)
                {
                    list.Add(xmlIndentedNewLine);
                    list.Add(parameter);
                }
            }

            if (exceptions is not null)
            {
                foreach (var exception in exceptions)
                {
                    list.Add(xmlIndentedNewLine);
                    list.Add(exception);
                }
            }

            if (returns is not null)
            {
                list.Add(xmlIndentedNewLine);
                list.Add(returns);
            }

            // This is the trivia syntax for the entire doc
            return SyntaxFactory
                .DocumentationComment(list.ToArray());
        }
    }
}
