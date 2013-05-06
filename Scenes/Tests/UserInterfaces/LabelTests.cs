using System;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class LabelTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void SaveAndLoad(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				var label = CreateLabel(content);
				var scene = new Scene();
				scene.Add(label);
				MemoryStream stream = scene.SaveToMemoryStream();
				var loadedScene = (Scene)stream.CreateFromMemoryStream();
				CheckLabelsMatch(label, loadedScene.Find("Label") as Label);
				scene.Show(entitySystem, content, input);
			});
		}

		private static Label CreateLabel(ContentLoader content)
		{
			return new Label(content.Load<Image>("DeltaEngineLogo"), Centered)
			{
				Name = "Label",
				Sprite = { Color = Color.Red, RenderLayer = 5, Rotation = 45 }
			};
		}

		private static readonly Rectangle Centered = new Rectangle(0.4f, 0.4f, 0.1f, 0.1f);

		private static void CheckLabelsMatch(Label label1, Label label2)
		{
			Assert.AreEqual(label1.Sprite.Color, label2.Sprite.Color);
			Assert.AreEqual(label1.Sprite.DrawArea, label2.Sprite.DrawArea);
			Assert.IsTrue(label2.Sprite.Image.Name == "DeltaEngineLogo");
			Assert.AreEqual(label1.Sprite.Visibility, label2.Sprite.Visibility);
			Assert.AreEqual(label1.Sprite.RenderLayer, label2.Sprite.RenderLayer);
			Assert.AreEqual(label1.Sprite.Rotation, label2.Sprite.Rotation);
		}
	}
}