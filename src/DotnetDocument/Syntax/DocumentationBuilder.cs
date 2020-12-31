using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Syntax
{
    public class DocumentationBuilder<T> where T : SyntaxNode
    {
        private T _node;
        private readonly List<string> _summaryLines = new();
        private string _returnsDescription;
        private readonly List<(string exception, string description)> _exceptions = new();
        private readonly List<(string name, string description)> _paramList = new();
        private readonly List<(string name, string description)> _typeParamList = new();

        public DocumentationBuilder<T> For(T node)
        {
            _node = node;
            return this;
        }

        public DocumentationBuilder<T> WithSummary(params string[] lines)
        {
            _summaryLines.AddRange(lines);
            return this;
        }

        public DocumentationBuilder<T> WithReturns(string returns)
        {
            _returnsDescription = returns;
            return this;
        }

        public DocumentationBuilder<T> WithException(string exception, string description)
        {
            _exceptions.Add((exception, description));
            return this;
        }

        public DocumentationBuilder<T> WithParam(string name, string description)
        {
            _paramList.Add((name, description));
            return this;
        }

        public DocumentationBuilder<T> WithTypeParam(string name, string description)
        {
            _typeParamList.Add((name, description));
            return this;
        }

        public T Build()
        {
            XmlElementSyntax returnsXmlElement = null;
            var typeParamElements = new List<XmlElementSyntax>();
            var paramElements = new List<XmlElementSyntax>();
            var exceptionsElements = new List<XmlElementSyntax>();

            var leadingTrivia = _node.GetLeadingTrivia();

            var indentationTrivia = DocumentationUtils
                .GetIndentationTrivia(_node);

            var xmlNewLine = DocumentationFactory
                .XmlNewLineToken(indentationTrivia);

            // Declare the summary XML element
            var summaryXmlElement = DocumentationFactory.Summary(_summaryLines, xmlNewLine, false);

            foreach (var typeParam in _typeParamList)
            {
                // Declare type param
                typeParamElements.Add(DocumentationFactory
                    .TypeParam(typeParam.name, typeParam.description));
            }

            foreach (var param in _paramList)
            {
                // Declare param
                paramElements.Add(DocumentationFactory
                    .Param(param.name, param.description));
            }

            if (!string.IsNullOrWhiteSpace(_returnsDescription))
            {
                // Declare the returns XML element
                returnsXmlElement = DocumentationFactory.Returns(_returnsDescription);
            }

            foreach (var exception in _exceptions)
            {
                // Declare the returns XML element
                exceptionsElements.Add(DocumentationFactory
                    .Exception(exception.exception, exception.description));
            }

            // Declare new line node 
            var newLineXmlNode = SyntaxFactory
                .XmlText(xmlNewLine);

            // This is the trivia syntax for the entire doc
            var docCommentTriviaSyntax = DocumentationFactory.XmlDocument(
                newLineXmlNode,
                summaryXmlElement,
                typeParamElements,
                paramElements,
                exceptionsElements,
                returnsXmlElement);

            var documentationTrivia = SyntaxFactory.Trivia(docCommentTriviaSyntax);

            // TODO: Research this
            var endOfLineTrivia = SyntaxFactory.EndOfLine(Environment.NewLine);

            var newLeadingTrivia = leadingTrivia
                .Add(documentationTrivia)
                .Add(endOfLineTrivia)
                .Add(indentationTrivia);

            var documentedNode = _node
                .WithLeadingTrivia(newLeadingTrivia);

            return documentedNode;
        }
    }
}
