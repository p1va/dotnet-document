using System;
using System.Collections.Generic;
using DotnetDocument.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotnetDocument.Syntax
{
    public class DocumentationBuilder<T> where T : SyntaxNode
    {
        private T? _node;
        private readonly List<string> _summaryLines = new();
        private bool _hasReturns;
        private string? _returnsDescription;
        private readonly List<(string exception, string description)> _exceptions = new();
        private readonly List<(string name, string description)> _paramList = new();
        private readonly List<(string name, string description)> _typeParamList = new();

        public DocumentationBuilder<T> For(T node)
        {
            _node = OnlyWhen.NotNull(node, nameof(node));

            return this;
        }

        public DocumentationBuilder<T> WithSummary(params string[] lines)
        {
            _summaryLines.AddRange(OnlyWhen.NotNull(lines, nameof(lines)));

            return this;
        }

        public DocumentationBuilder<T> WithSummary(IEnumerable<string> lines)
        {
            _summaryLines.AddRange(OnlyWhen.NotNull(lines, nameof(lines)));

            return this;
        }

        public DocumentationBuilder<T> WithReturns(string returns)
        {
            _hasReturns = true;
            _returnsDescription = OnlyWhen.NotNull(returns, nameof(returns));

            return this;
        }

        public DocumentationBuilder<T> WithException(string exception, string description)
        {
            _exceptions.Add((
                OnlyWhen.NotNull(exception, nameof(exception)),
                OnlyWhen.NotNull(description, nameof(description))));

            return this;
        }

        public DocumentationBuilder<T> WithExceptions((string exception, string description)[] exceptions)
        {
            foreach (var exception in exceptions)
            {
                WithException(exception.exception, exception.description);
            }

            return this;
        }

        public DocumentationBuilder<T> WithExceptions(IEnumerable<(string exception, string description)> exceptions)
        {
            foreach (var exception in exceptions)
            {
                WithException(exception.exception, exception.description);
            }

            return this;
        }

        public DocumentationBuilder<T> WithParam(string name, string description)
        {
            _paramList.Add((
                OnlyWhen.NotNull(name, nameof(name)),
                OnlyWhen.NotNull(description, nameof(description))));

            return this;
        }

        public DocumentationBuilder<T> WithParams(IEnumerable<(string name, string description)> @params)
        {
            foreach (var param in @params)
            {
                WithParam(param.name, param.description);
            }

            return this;
        }

        public DocumentationBuilder<T> WithTypeParam(string name, string description)
        {
            _typeParamList.Add((
                OnlyWhen.NotNull(name, nameof(name)),
                OnlyWhen.NotNull(description, nameof(description))));

            return this;
        }

        public DocumentationBuilder<T> WithTypeParams(IEnumerable<(string name, string description)> typeParams)
        {
            foreach (var typeParam in typeParams)
            {
                WithTypeParam(typeParam.name, typeParam.description);
            }

            return this;
        }

        public T Build()
        {
            // Ensure a node was provided
            var node = OnlyWhen.NotNull(_node, "node");

            XmlElementSyntax? returnsXmlElement = null;
            var typeParamElements = new List<XmlElementSyntax>();
            var paramElements = new List<XmlElementSyntax>();
            var exceptionsElements = new List<XmlElementSyntax>();

            var leadingTrivia = node.GetLeadingTrivia();

            var indentationTrivia = SyntaxUtils
                .GetIndentationTrivia(node);

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

            if (_hasReturns)
            {
                // Declare the returns XML element
                returnsXmlElement = DocumentationFactory.Returns(_returnsDescription ?? string.Empty);
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
            var docCommentTriviaSyntax = DocumentationFactory.XmlDocument(newLineXmlNode,
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

            var documentedNode = node
                .WithLeadingTrivia(newLeadingTrivia);

            return documentedNode;
        }
    }
}
