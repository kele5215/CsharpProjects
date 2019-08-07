namespace ConvertSimpTrad
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// Represents a set of localized resources.
    /// </summary>
    [Localizable(false)]
    public class ResourceLanguage
    {
        private const string Quote = "\"";
        private const string WinFormsMemberNamePrefix = @">>";

        private static readonly XName _spaceAttributeName = XNamespace.Xml.GetName(@"space");
        private static readonly XName _typeAttributeName = XNamespace.None.GetName(@"type");
        private static readonly XName _mimetypeAttributeName = XNamespace.None.GetName(@"mimetype");
        private static readonly XName _nameAttributeName = XNamespace.None.GetName(@"name");

        private readonly XDocument _document;

        private readonly XElement _documentRoot;

        private readonly string _filePath;

        private IDictionary<string, Node> _nodes = new Dictionary<string, Node>();

        private readonly XName _dataNodeName;

        private readonly XName _valueNodeName;

        private readonly XName _commentNodeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceLanguage" /> class.
        /// </summary>
        /// <param name="filePath">The .resx file having all the localization.</param>
        /// </exception>
        internal ResourceLanguage(string filePath)
        {

            Contract.Requires(filePath != null);

            _filePath = filePath;

            try
            {
                _document = Load(_filePath);
                _documentRoot = _document.Root;
            }
            catch (XmlException ex)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, filePath), ex);
            }

            if (_documentRoot == null)
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, filePath));

            var defaultNamespace = _documentRoot.GetDefaultNamespace();

            _dataNodeName = defaultNamespace.GetName(@"data");
            _valueNodeName = defaultNamespace.GetName(@"value");
            _commentNodeName = defaultNamespace.GetName(@"comment");

            UpdateNodes(filePath);
        }

        public XDocument Load(string filePath)
        {
            Contract.Ensures(Contract.Result<XDocument>() != null);

            var document = XDocument.Load(filePath);

            return document;
        }

        private void UpdateNodes(string filePath)
        {
            Contract.Requires(filePath != null);

            var data = _documentRoot.Elements(_dataNodeName);

            var elements = data
                .Where(IsStringType)
                .Select(item => new Node(this, item))
                .Where(item => !item.Key.StartsWith(WinFormsMemberNamePrefix, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (elements.Any(item => string.IsNullOrEmpty(item.Key)))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, filePath));

            try
            {
                _nodes = elements.ToDictionary(item => item.Key);
            }
            catch (ArgumentException ex)
            {
                var duplicateKeys = string.Join(@", ", elements.GroupBy(item => item.Key).Where(group => group.Count() > 1).Select(group => Quote + group.Key + Quote));
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, filePath, duplicateKeys), ex);
            }
        }

        /// <summary>
        /// Gets all the resource keys defined in this language.
        /// </summary>
        [ContractVerification(false)]
        public IEnumerable<string> ResourceKeys
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<string>>(), item => item != null));

                return _nodes.Keys;
            }
        }

        internal string GetValue(string key)
        {
            Contract.Requires(key != null);

            Node node;

            return !_nodes.TryGetValue(key, out node) ? null : node.Text;
        }

        internal bool SetValue(string key, string value)
        {
            Contract.Requires(key != null);

            return GetValue(key) == value || SetNodeData(key, node => node.Text = value);
        }

        private bool SetNodeData(string key, Action<Node> updateCallback)
        {
            Contract.Requires(key != null);
            Contract.Requires(updateCallback != null);

            try
            {
                Node node;

                if (!_nodes.TryGetValue(key, out node) || (node == null))
                {
                    node = CreateNode(key);
                }

                updateCallback(node);

                if (string.IsNullOrEmpty(node.Text) && string.IsNullOrEmpty(node.Comment))
                {
                    node.Element.Remove();
                    _nodes.Remove(key);
                }

                return true;
            }
            catch (Exception ex)
            {
                var message = string.Format(CultureInfo.CurrentCulture, _filePath, ex.Message);
                throw new IOException(message, ex);
            }
        }

        public void Save()
        {
            var document = _document;

            if (document == null)
                return;

            InternalSave(document);


        }

        protected virtual void InternalSave( XDocument document)
        {
            Contract.Requires(document != null);

            document.Save(_filePath);
        }

        public string FileName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return new FileInfo(_filePath).Name;
            }
        }

        public string FileFullName
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return _filePath;
            }
        }

        private static bool IsStringType(XElement entry)
        {
            Contract.Requires(entry != null);

            var typeAttribute = entry.Attribute(_typeAttributeName);

            if (typeAttribute != null)
            {
                return string.IsNullOrEmpty(typeAttribute.Value) || typeAttribute.Value.StartsWith(typeof(string).Name, StringComparison.OrdinalIgnoreCase);
            }

            var mimeTypeAttribute = entry.Attribute(_mimetypeAttributeName);

            return mimeTypeAttribute == null;
        }


        internal string GetComment(string key)
        {
            Contract.Requires(key != null);

            Node node;

            if (!_nodes.TryGetValue(key, out node) || (node == null))
                return null;

            return node.Comment;
        }

        private Node CreateNode(string key)
        {
            Contract.Requires(key != null);
            Contract.Ensures(Contract.Result<Node>() != null);

            Node node;
            var content = new XElement(_valueNodeName);
            content.Add(new XText(string.Empty));

            var entry = new XElement(_dataNodeName, new XAttribute(_nameAttributeName, key), new XAttribute(_spaceAttributeName, @"preserve"));
            entry.Add(content);

            _documentRoot.Add(entry);
            _nodes.Add(key, node = new Node(this, entry));
            return node;
        }

        internal bool KeyExists(string key)
        {
            Contract.Requires(key != null);

            return _nodes.ContainsKey(key);
        }


        private class Node
        {

            private readonly ResourceLanguage _owner;

            private readonly XElement _element;
            private string _text;
            private string _comment;

            public Node(ResourceLanguage owner, XElement element)
            {
                Contract.Requires(owner != null);
                Contract.Requires(element != null);
                Contract.Requires(owner._commentNodeName != null);

                _element = element;
                _owner = owner;
            }


            public XElement Element
            {
                get
                {
                    Contract.Ensures(Contract.Result<XElement>() != null);

                    return _element;
                }
            }


            public string Key
            {
                get
                {
                    Contract.Ensures(Contract.Result<string>() != null);

                    return GetNameAttribute(_element).Value;
                }
                set
                {
                    Contract.Requires(value != null);

                    GetNameAttribute(_element).Value = value;
                }
            }

            public string Text
            {
                get
                {
                    return _text ?? (_text = LoadText());
                }
                set
                {
                    _text = value ?? string.Empty;

                    var entry = Element;

                    var valueElement = entry.Element(_owner._valueNodeName);
                    if (valueElement == null)
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, _owner.FileName));

                    if (valueElement.FirstNode == null)
                    {
                        valueElement.Add(value);
                    }
                    else
                    {
                        valueElement.FirstNode.ReplaceWith(value);
                    }
                }
            }

            public string Comment
            {
                get
                {
                    return _comment ?? (_comment = LoadComment());
                }
                set
                {
                    _comment = value ?? string.Empty;

                    var entry = Element;

                    var valueElement = entry.Element(_owner._commentNodeName);

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        if (valueElement != null)
                        {
                            valueElement.Remove();
                        }
                    }
                    else
                    {
                        if (valueElement == null)
                        {
                            valueElement = new XElement(_owner._commentNodeName);
                            entry.Add(valueElement);
                        }

                        var textNode = valueElement.FirstNode as XText;
                        if (textNode == null)
                        {
                            textNode = new XText(value);
                            valueElement.Add(textNode);
                        }
                        else
                        {
                            textNode.Value = value;
                        }
                    }
                }
            }

            private string LoadText()
            {
                var entry = Element;

                var valueElement = entry.Element(_owner._valueNodeName);
                if (valueElement == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, _owner.FileName));
                }

                var textNode = valueElement.FirstNode as XText;

                return textNode == null ? string.Empty : textNode.Value;
            }

            private string LoadComment()
            {
                var entry = Element;

                var valueElement = entry.Element(_owner._commentNodeName);
                if (valueElement == null)
                    return string.Empty;

                var textNode = valueElement.FirstNode as XText;

                return textNode == null ? string.Empty : textNode.Value;
            }


            private XAttribute GetNameAttribute(XElement entry)
            {
                Contract.Requires(entry != null);
                Contract.Ensures(Contract.Result<XAttribute>() != null);

                var nameAttribute = entry.Attribute(_nameAttributeName);
                if (nameAttribute == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, _owner._filePath));
                }

                return nameAttribute;
            }

            [ContractInvariantMethod]
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
            [Conditional("CONTRACTS_FULL")]
            private void ObjectInvariant()
            {
                Contract.Invariant(_element != null);
                Contract.Invariant(_owner != null);
                Contract.Invariant(_owner._commentNodeName != null);
            }
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        [Conditional("CONTRACTS_FULL")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_document != null);
            Contract.Invariant(_documentRoot != null);
            Contract.Invariant(_filePath != null);
            Contract.Invariant(_nodes != null);
            Contract.Invariant(_dataNodeName != null);
            Contract.Invariant(_valueNodeName != null);
            Contract.Invariant(_commentNodeName != null);
        }
    }
}
