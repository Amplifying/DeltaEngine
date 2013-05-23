using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	public class SceneFileTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void CreateSaveLoadAndShowScene(Type resolverType)
		{
			Start(resolverType,
				(EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
				{
					var originalScene = CreateAndSaveScene(content);
					var loadedScene = LoadAndShowScene(entitySystem, content, input);
					var originalLabel = (Label)originalScene.Find("Label");
					var loadedLabel = (Label)originalScene.Find("Label");
					Assert.AreEqual(originalLabel.Sprite.DrawArea, loadedLabel.Sprite.DrawArea);
					var originalButton = (Button)originalScene.Find("Button");
					var loadedButton = (Button)loadedScene.Find("Button");
					Assert.AreEqual(originalButton.PressedColor, loadedButton.PressedColor);
				});
		}

		private static Scene CreateAndSaveScene(ContentLoader content)
		{
			var logo = content.Load<Image>("DeltaEngineLogo");
			var scene = new Scene();
			scene.Add(new Label(logo, new Rectangle(0.45f, 0.3f, 0.1f, 0.1f)) { Name = "Label" });
			scene.Add(new Button(logo, new Rectangle(0.4f, 0.4f, 0.2f, 0.2f))
			{
				Name = "Button",
				PressedColor = Color.Red,
				NormalColor = Color.Blue
			});
			new SceneFile(scene).Save("Scene");
			return scene;
		}

		private static Scene LoadAndShowScene(EntitySystem entitySystem, ContentLoader content,
			InputCommands input)
		{
			var scene = new SceneFile("Scene").Scene;
			scene.Show(entitySystem, content, input);
			return scene;
		}
	}
}