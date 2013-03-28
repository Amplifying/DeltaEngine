using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.UserInterfaces;
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
			this.demoImage = demoImage;
		}

		private readonly Image demoImage;

		public override void Register()
		{
			SetupVectorText();
			SetupXmlContent();
			SetupSceneContent();			
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
			var character = new XmlData("Char" + i);
			vectorTextData.AddChild(character);
			character.AddAttribute("Character", (char)i);
			character.AddAttribute("Lines", "(0,0)-(1,1)");
		}

		private void SetupXmlContent()
		{
			var mockXmlContent = new Mock<XmlContent>("TestXml");
			mockXmlContent.SetupGet(c => c.XmlData).Returns(vectorTextData);
			testResolver.RegisterMock(mockXmlContent.Object);
		}

		private void SetupSceneContent()
		{
			Scene scene = CreateSceneContent();
			var mockSceneContent = new Mock<SceneContent>("dummy");
			mockSceneContent.SetupGet(c => c.Scene).Returns(scene);
			testResolver.RegisterMock(mockSceneContent.Object);
		}

		private Scene CreateSceneContent()
		{
			var scene = new Scene();
			scene.Add(new Label(demoImage, new Rectangle(0.45f, 0.3f, 0.1f, 0.1f)) { Name = "Label" });
			scene.Add(new Button(demoImage, new Rectangle(0.4f, 0.4f, 0.2f, 0.2f))
			{
				Name = "Button",
				NormalColor = Color.Red,
				MouseoverColor = Color.Green,
				PressedColor = Color.Blue
			});

			return scene;
		}
	}
}