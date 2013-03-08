using System;
using System.Linq;

namespace DeltaEngine.Core.Xml
{
	/// <summary>
	/// This handles converting a string of Xml data into an XmlData object
	/// </summary>
	public class XmlTranscriber
	{
		public XmlTranscriber(string xml)
		{
			if (xml == null)
				throw new InvalidXmlStringException();

			xmlLines = xml.Split('>');
			ParseXmlLines();

			if (Result == null)
				throw new InvalidXmlStringException();
		}

		public XmlData Result { get; private set; }

		public class InvalidXmlStringException : Exception {}

		private readonly string[] xmlLines;

		private void ParseXmlLines()
		{
			lineNo = -1;

			while (lineNo < xmlLines.Count() - 1)
				ParseNextXmlLine();
		}

		private int lineNo;

		private void ParseNextXmlLine()
		{
			lineNo++;
			line = xmlLines[lineNo].Trim();

			if (IgnoreLine())
				return;

			if (line.Substring(0, 2) == "</")
				CompleteChild();
			else if (line.Substring(line.Length - 1, 1) == "/")
				CreateChild();
			else
				BeginChild();
		}

		private string line;

		private bool IgnoreLine()
		{
			return line.Length < 3 || line.Substring(0, 1) != "<" || line.Substring(1, 1) == "?";
		}

		private void CompleteChild()
		{
			if (Result.Parent != null)
				Result = Result.Parent;
		}

		private void CreateChild()
		{
			BeginChild();
			CompleteChild();
		}

		private void BeginChild()
		{
			var name = GetUpTo(' ');
			Result = new XmlData(name, Result);
			AddAttributes();
		}

		private string GetUpTo(char c)
		{
			int pos = line.IndexOf(c);
			if (pos <= 0)
				return GetWholeLine();

			string word = line.Substring(0, pos);
			line = pos < line.Length ? line.Substring(pos + 1) : "";
			return RemoveUnwantedChars(word);
		}

		private string GetWholeLine()
		{
			string word = line;
			line = "";
			return RemoveUnwantedChars(word);
		}

		private static string RemoveUnwantedChars(string word)
		{
			word = word.Replace(">", "");
			word = word.Replace("<", "");
			word = word.Replace("\"", "");
			word = word.Replace("\n", "");
			word = word.Replace("\r", "");
			word = word.Replace("\t", "");
			return word;
		}

		private void AddAttributes()
		{
			while (line != "")
				AddAttribute();
		}

		private void AddAttribute()
		{
			string attribute = GetUpTo('"').Trim(' ').Trim('=');
			string value = GetUpTo('"');
			if (attribute != "" && attribute != "/")
				Result.AddAttribute(attribute, value);
		}
	}
}