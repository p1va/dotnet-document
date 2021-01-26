using System.Collections.Generic;
using System.Linq;
using DotnetDocument.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Syntax
{
    public static class DocumentationFactory
    {
        public static SyntaxToken XmlNewLineToken(SyntaxTrivia indentationTrivia) => SyntaxFactory
            .XmlTextNewLine("\n", false)
            .WithTrailingTrivia(SyntaxFactory.TriviaList(indentationTrivia, SyntaxFactory
                .DocumentationCommentExterior("/// ")));

        public static XmlElementSyntax Summary(IEnumerable<string> summaryLines, SyntaxToken xmlIndentedNewLine,
            bool keepSameLine)
        {
            var xmlSummaryLines = new List<XmlNodeSyntax>();

            xmlSummaryLines.Add(SyntaxFactory
                .XmlText(SyntaxFactory.TokenList(xmlIndentedNewLine)));

            foreach (var summaryLine in summaryLines)
            {
                if (summaryLine.Contains("<<") && summaryLine.Contains(">>"))
                {
                    // Get class name
                    var className = summaryLine.SubstringBetween("<<", ">>");

                    if (!string.IsNullOrWhiteSpace(className))
                    {
                        var beforeToken = summaryLine.Split("<<").FirstOrDefault() ?? string.Empty;
                        var afterToken = summaryLine.Split(">>").LastOrDefault() ?? string.Empty;

                        var seeElement = See(className);

                        var beforeSeeElement = SyntaxFactory
                            .XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(beforeToken)));

                        var afterSeeElement = SyntaxFactory
                            .XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(afterToken)));

                        xmlSummaryLines.Add(beforeSeeElement);
                        xmlSummaryLines.Add(seeElement);
                        xmlSummaryLines.Add(afterSeeElement);
                    }
                }
                else
                {
                    xmlSummaryLines.Add(SyntaxFactory
                        .XmlText(SyntaxFactory.TokenList(SyntaxFactory.XmlTextLiteral(summaryLine))));
                }

                xmlSummaryLines.Add(SyntaxFactory
                    .XmlText(SyntaxFactory.TokenList(xmlIndentedNewLine)));
            }

            // Declare the summary XML element
            return SyntaxFactory.XmlSummaryElement(xmlSummaryLines.ToArray());
        }

        public static XmlElementSyntax Returns(string description) => SyntaxFactory
            .XmlReturnsElement(SyntaxFactory
                .XmlText(description));

        public static XmlElementSyntax Exception(string exception, string description) => SyntaxFactory
            .XmlExceptionElement(SyntaxFactory
                .TypeCref(SyntaxFactory.ParseTypeName(exception)), SyntaxFactory
                .XmlText(description));

        public static XmlEmptyElementSyntax See(string type) => SyntaxFactory
            .XmlSeeElement(SyntaxFactory
                .TypeCref(SyntaxFactory
                    .ParseTypeName(type)));

        public static XmlEmptyElementSyntax SeeAlso(string type) => SyntaxFactory
            .XmlSeeAlsoElement(SyntaxFactory
                .TypeCref(SyntaxFactory
                    .ParseTypeName(type)));

        public static XmlElementSyntax Param(string name, string description) => SyntaxFactory
            .XmlParamElement(name, SyntaxFactory
                .XmlText(description));

        public static XmlElementSyntax TypeParam(string type, string description)
        {
            // Declare type params
            var attributeList = SyntaxFactory.List<XmlAttributeSyntax>()
                .Add(SyntaxFactory
                    .XmlNameAttribute(SyntaxFactory.XmlName(" name"),
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
            List<XmlEmptyElementSyntax>? seeAlsos = null,
            List<XmlElementSyntax>? typeParameters = null,
            List<XmlElementSyntax>? parameters = null,
            List<XmlElementSyntax>? exceptions = null,
            XmlElementSyntax? returns = null)
        {
            var list = new List<XmlNodeSyntax>()
            {
                summary
            };

            // Add each see also to list
            seeAlsos?.ForEach(seeAlso =>
            {
                list.Add(xmlIndentedNewLine);
                list.Add(seeAlso);
            });

            // Add each type param to list
            typeParameters?.ForEach(typeParam =>
            {
                list.Add(xmlIndentedNewLine);
                list.Add(typeParam);
            });

            // Add each type param to list
            parameters?.ForEach(param =>
            {
                list.Add(xmlIndentedNewLine);
                list.Add(param);
            });

            // Add each exception to list
            exceptions?.ForEach(exception =>
            {
                list.Add(xmlIndentedNewLine);
                list.Add(exception);
            });

            if (returns is not null)
            {
                list.Add(xmlIndentedNewLine);
                list.Add(returns);
            }

            // This is the trivia syntax for the entire doc
            return SyntaxFactory.DocumentationComment(list.ToArray());
        }
    }
}
