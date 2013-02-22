using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Core.Xml
{
	/// <summary>
	/// This holds Xml data as a tree structure - with potentially one parent and many children. 
	/// At every level it can hold a list of xml attributes which are name/value pairings
	/// </summary>
	public class XmlData
	{
		public XmlData(string name)
		{
			Name = name;
			Children = new List<XmlData>();
			Attributes = new List<XmlAttribute>();
		}

		public string Name { get; set; }
		public List<XmlData> Children { get; private set; }
		public List<XmlAttribute> Attributes { get; private set; }

		public XmlData(string name, XmlData parent)
			: this(name)
		{
			Parent = parent;
			if (parent != null)
				parent.Children.Add(this);
		}

		public XmlData Parent { get; private set; }

		public XmlData GetChild(string name)
		{
			return Children.FirstOrDefault(child => child.Name == name);
		}

		public List<XmlData> GetChildren(string name)
		{
			return Children.Where(child => child.Name == name).ToList();
		}

		public void AddAttribute(string name, object value)
		{
			Attributes.Add(new XmlAttribute(name, value));
		}

		/// <summary>
		/// Removes all attributes whose attribute name matches the given name
		/// </summary>
		public void RemoveAttributes(string name)
		{
			Attributes.RemoveAll(attribute => attribute.Name == name);
		}

		public string GetValue(string name)
		{
			return Attributes.FirstOrDefault(attribute => attribute.Name == name).Value;
		}

		public string ToXmlString()
		{
			return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + XmlBody(0);
		}

		internal string XmlBody(int tabDepth)
		{
			return Children.Count > 0 ? XmlBodyWithChildren(tabDepth) : XmlBodyWithoutChildren(tabDepth);
		}

		private string XmlBodyWithChildren(int tabDepth)
		{
			string xml = "\r\n" + XmlAttributesString(tabDepth) + ">";

			foreach (XmlData child in Children)
				xml += child.XmlBody(tabDepth + 1);

			xml += "\r\n" + new string("\t"[0], tabDepth) + "</" + Name + ">";
			return xml;
		}

		private string XmlBodyWithoutChildren(int tabDepth)
		{
			return "\r\n" + XmlAttributesString(tabDepth) + " />";
		}

		private string XmlAttributesString(int tabDepth)
		{
			string xml = new string("\t"[0], tabDepth) + "<" + Name;

			foreach (XmlAttribute attribute in Attributes)
				xml += " " + attribute.Name + "=\"" + attribute.Value + "\"";

			return xml;
		}
	}
}