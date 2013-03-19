using DeltaEngine.Core.Xml;
using DeltaEngine.Graphics;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Mocks common Content objects for testing
	/// </summary>
	public class TestContentResolver : TestModuleResolver
	{
		public TestContentResolver(TestResolver testResolver, Image demoImage)
			: base(testResolver)
		{
			SetupVectorText();
			SetupXmlContent();
		}

		private void SetupVectorText()
		{
			vectorTextData = new XmlData("VectorText");
			for (int i = 'A'; i <= 'Z'; i++)
				AddCharacter(i);

			for (int i = '0'; i <= '9'; i++)
				AddCharacter(i);

			AddCharacter('.');
			testResolver.RegisterMock(vectorTextData);
		}

		private XmlData vectorTextData;

		private void AddCharacter(int i)
		{
			var character = new XmlData("Char" + i, vectorTextData);
			character.AddAttribute("Character", (char)i);
			character.AddAttribute("Lines", "(0,0)-(1,1)");
		}

		private void SetupXmlContent()
		{
			var mockXmlContent = new Mock<XmlContent>("dummy");
			mockXmlContent.SetupGet(c => c.XmlData).Returns(vectorTextData);
			testResolver.RegisterMock(mockXmlContent.Object);
		}
	}
}
