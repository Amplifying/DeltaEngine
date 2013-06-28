using System.IO;
using DeltaEngine.Content.Xml;

namespace DeltaEngine.Platforms.Mocks
{
	internal class MockXmlContent : XmlContent
	{
		public MockXmlContent(string contentName)
			: base(contentName) { }

		protected override void LoadData(Stream fileData)
		{
			if (Name == "VectorText")
			{
				if (vectorTextData == null)
					SetupVectorText();
				Data = vectorTextData;
				return;
			}
			Data = new XmlData("Root");
			Data.AddChild(new XmlData("Hi"));
		}

		private static void SetupVectorText()
		{
			vectorTextData = new XmlData("VectorText");
			for (int i = 'A'; i <= 'Z'; i++)
				AddCharacter(i);

			for (int i = '0'; i <= '9'; i++)
				AddCharacter(i);

			AddCharacter('.');
		}

		private static XmlData vectorTextData;

		private static void AddCharacter(int i)
		{
			var character = new XmlData("Char" + i);
			vectorTextData.AddChild(character);
			character.AddAttribute("Character", (char)i);
			character.AddAttribute("Lines", "(0,0)-(1,1)");
		}
	}
}