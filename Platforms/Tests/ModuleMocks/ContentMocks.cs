using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Scenes;
using DeltaEngine.Logging;
using DeltaEngine.Scenes.UserInterfaces;

namespace DeltaEngine.Platforms.Tests.ModuleMocks
{
	/// <summary>
	/// Mocks common Content objects for testing
	/// </summary>
	public class ContentMocks : BaseMocks
	{
		internal ContentMocks(AutofacStarterForMockResolver resolver)
			: base(resolver)
		{
			SetupVectorText();
			SetupXmlContent();
			//TODO: Mock Scenes
			//SetupSceneContent();		
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

		private void SetupXmlContent()
		{
			resolver.Register<MockXmlContent>();
		}

		public class MockXmlContent : XmlContent
		{
			public MockXmlContent(string filename, Logger log)
				: base(filename, log) { }

			protected override void LoadData(Stream fileData)
			{
				if (Name == "VectorText")
				{
					Data = vectorTextData;
					return;
				}
				Data = new XmlData("Root");
				Data.AddChild(new XmlData("Hi"));
			}
		}

		//TODO: Mock Scenes
		//private void SetupSceneContent()
		//{
		//	resolver.Register<MockSceneContent>();
		//	resolver.Register<MockScene>();
		//}

		//[IgnoreForResolver]
		//public class MockScene : Scene
		//{
		//	public MockScene(ContentLoader content)
		//	{
		//		var demoImage = content.Load<Image>("test");
		//		Add(new Label(demoImage, new Rectangle(0.45f, 0.3f, 0.1f, 0.1f)) { Name = "Label" });
		//		Add(new Button(demoImage, new Rectangle(0.4f, 0.4f, 0.2f, 0.2f))
		//		{
		//			Name = "Button",
		//			NormalColor = Color.Red,
		//			MouseoverColor = Color.Green,
		//			PressedColor = Color.Blue
		//		});
		//	}
		//}

		//public class MockSceneContent : SceneContent
		//{
		//	public MockSceneContent(string contentName, MockScene scene)
		//		: base(contentName)
		//	{
		//		Scene = scene;
		//	}

		//	protected override void LoadData(Stream fileData) {}
		//	protected override void DisposeData() {}
		//}
	}
}