using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Content
{
	public class ContentMetaData
	{
		public ContentMetaData(string name, ContentType type)
		{
			Values = new Dictionary<string, string>();
			Name = name;
			Type = type;
		}

		public ContentMetaData(IEnumerable<XAttribute> attributes)
		{
			Values = new Dictionary<string, string>();
			foreach (var attribute in attributes)
				switch (attribute.Name.LocalName)
				{
				case "Name":
					Name = attribute.Value;
					break;
				case "Type":
					Type = EnumExtensions.Parse<ContentType>(attribute.Value);
					break;
				case "LastTimeUpdated":
					LastTimeUpdated = Convert.ToDateTime(attribute.Value);
					break;
				case "LocalFilePath":
					LocalFilePath = attribute.Value;
					break;
				case "PlatformFileId":
					PlatformFileId = attribute.Value.Convert<int>();
					break;
				case "FileSize":
					FileSize = attribute.Value.Convert<int>();
					break;
				default:
					Values.Add(attribute.Name.LocalName, attribute.Value);
					break;
				}

			if (string.IsNullOrEmpty(Name))
				throw new InvalidContentMetaDataNameIsAlwaysNeeded();
		}

		public string Name { get; private set; }
		public ContentType Type { get; private set; }
		public DateTime LastTimeUpdated { get; private set; }
		public string LocalFilePath { get; set; }
		public int PlatformFileId { get; private set; }
		public int FileSize { get; private set; }
		internal Dictionary<string, string> Values { get; set; }
		public class InvalidContentMetaDataNameIsAlwaysNeeded : Exception {}
		private readonly List<ContentMetaData> children = new List<ContentMetaData>();

		public T Get<T>(string attributeName, T defaultValue)
		{
			return Values.ContainsKey(attributeName) ? Values[attributeName].Convert<T>() : defaultValue;
		}

		public List<string> GetChildrenNames(ContentType type)
		{
			return (from child in children where child.Type == type select child.Name).ToList();
		}

		internal void AddChildren(ContentMetaData child)
		{
			children.Add(child);
		}
	}
}