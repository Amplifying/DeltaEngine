using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DeltaEngine.Core.Xml
{
	/// <summary>
	/// Simplified Xml handling
	/// </summary>
	public class XmlData
	{
		protected XmlData()
		{
			Children = new List<XmlData>();
			Attributes = new List<XmlAttribute>();
		}

		public List<XmlData> Children { get; private set; }
		public List<XmlAttribute> Attributes { get; private set; }

		public XmlData(string name)
			: this()
		{
			Name = name;
		}

		public string Name
		{
			get { return name; }
			set
			{
				if (string.IsNullOrEmpty(value) || value.Contains(" "))
					throw new InvalidXmlNameException();

				name = value;
			}
		}

		public class InvalidXmlNameException : Exception {}

		private string name;

		internal XmlData(XElement root)
			: this()
		{
			Name = root.Name.LocalName;
			Value = string.Concat(root.Nodes().OfType<XText>().Select(t => t.Value));
			InitializeAttributes(root);
			InitializeChildren(root);
		}

		public XmlData Parent { get; private set; }
		public string Value { get; set; }

		private void InitializeAttributes(XElement root)
		{
			var attributes = new List<XAttribute>(root.Attributes());
			foreach (XAttribute attribute in attributes)
				Attributes.Add(new XmlAttribute(attribute.Name.LocalName, attribute.Value));
		}

		private void InitializeChildren(XElement root)
		{
			var children = new List<XElement>(root.Elements());
			foreach (XElement childXElement in children)
				AddChild(new XmlData(childXElement));
		}

		public XmlData AddChild(XmlData child)
		{
			child.Parent = this;
			Children.Add(child);
			return this;
		}

		public void AddAttribute(string attribute, char value)
		{
			Attributes.Add(new XmlAttribute(attribute, value));
		}

		public void AddAttribute(string attribute, float value)
		{
			Attributes.Add(new XmlAttribute(attribute, value));
		}

		public void AddAttribute(string attribute, double value)
		{
			Attributes.Add(new XmlAttribute(attribute, value));
		}

		public void AddAttribute(string attribute, object value)
		{
			Attributes.Add(new XmlAttribute(attribute, value));
		}

		public string GetValue(string attributeName)
		{
			foreach (XmlAttribute attribute in Attributes)
				if (attribute.Name == attributeName)
					return attribute.Value;

			return "";
		}

		public string GetDescendantValue(string attributeName)
		{
			XmlAttribute? attribute = FindFirstDescendantAttribute(Children, attributeName);
			return attribute == null ? "" : ((XmlAttribute)attribute).Value;
		}

		private static XmlAttribute? FindFirstDescendantAttribute(IEnumerable<XmlData> children,
			string attributeName)
		{
			foreach (XmlData child in children)
			{
				foreach (XmlAttribute attribute in child.Attributes)
					if (attribute.Name == attributeName)
						return attribute;

				XmlAttribute? childAttribute = FindFirstDescendantAttribute(child.Children, attributeName);
				if (childAttribute != null)
					return childAttribute;
			}

			return null;
		}

		public Dictionary<string, string> GetAttributes()
		{
			var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
			foreach (XmlAttribute attribute in Attributes)
				result.Add(attribute.Name, attribute.Value);

			return result;
		}

		public XmlData GetChild(string childName)
		{
			return Children.FirstOrDefault(child => child.Name.Compare(childName));
		}

		public List<XmlData> GetChildren(string childName)
		{
			return Children.Where(child => child.Name.Compare(childName)).ToList();
		}

		public XmlData GetDescendant(string childName)
		{
			foreach (XmlData child in Children)
			{
				if (child.Name.Compare(childName))
					return child;

				XmlData childOfChild = child.GetChild(childName);
				if (childOfChild != null)
					return childOfChild;
			}

			return null;
		}

		public XmlData GetDescendant(XmlAttribute attribute, string childName = null)
		{
			bool anyDescendant = String.IsNullOrEmpty(childName);
			foreach (XmlData child in Children)
			{
				if ((anyDescendant || child.Name.Compare(childName)) &&
					child.GetValue(attribute.Name) == attribute.Value)
					return child;

				XmlData childOfChild = child.GetDescendant(attribute, childName);
				if (childOfChild != null)
					return childOfChild;
			}

			return null;
		}

		public XmlData GetDescendant(List<XmlAttribute> attributes, string childName = null)
		{
			bool anyDescendant = String.IsNullOrEmpty(childName);
			foreach (XmlData child in Children)
			{
				if (anyDescendant || child.Name.Compare(childName))
					if (child.ContainsAttributes(attributes))
						return child;

				XmlData childOfChild = child.GetDescendant(attributes, childName);
				if (childOfChild != null)
					return childOfChild;
			}

			return null;
		}

		private bool ContainsAttributes(IEnumerable<XmlAttribute> attributes)
		{
			bool matches = true;
			foreach (var attribute in attributes)
				if (GetValue(attribute.Name) != attribute.Value)
					matches = false;

			return matches;
		}

		public int GetTotalNodeCount()
		{
			return 1 + Children.Sum(t => t.GetTotalNodeCount());
		}

		public void Remove()
		{
			if (Parent != null)
				Parent.RemoveChild(this);
		}

		public bool RemoveChild(XmlData child)
		{
			if (!Children.Contains(child))
				return false;

			Children.Remove(child);
			return true;
		}

		public void RemoveAttribute(string attributeName)
		{
			Attributes.RemoveAll(attribute => attribute.Name == attributeName);
		}

		public void ClearAttributes()
		{
			Attributes.Clear();
		}

		public override string ToString()
		{
			return "XmlData=" + Name + ": " + XRootElement.ToString().MaxStringLength(200);
		}

		public XElement XRootElement
		{
			get
			{
				var root = new XElement(Name);
				if (!string.IsNullOrEmpty(Value))
					root.Value = Value;

				XDocument doc = XDocumentHasHeader
					? new XDocument(new XDeclaration("1.0", "utf-8", null)) : new XDocument();
				doc.Add(root);
				AddXAttributes(root);
				AddXChildren(root, doc);
				return root;
			}
		}

		public bool XDocumentHasHeader { get; set; }

		private void AddXAttributes(XElement root)
		{
			foreach (XmlAttribute attribute in Attributes)
				root.SetAttributeValue(attribute.Name, attribute.Value);
		}

		private void AddXChildren(XContainer root, XDocument doc)
		{
			foreach (XmlData child in Children)
				root.Add(child.GetXRootElement(doc));
		}

		private XElement GetXRootElement(XDocument doc)
		{
			var root = new XElement(Name);
			if (!string.IsNullOrEmpty(Value))
				root.Value = Value;

			AddXAttributes(root);
			AddXChildren(root, doc);
			return root;
		}

		public string ToXmlString()
		{
			XDocument doc = XRootElement.Document;
			return doc == null ? "" : doc.ToString();
		}
	}
}