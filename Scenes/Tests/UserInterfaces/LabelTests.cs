using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class LabelTests : TestStarter
	{
		[VisualTest]
		public void SaveAndLoad(Type resolver)
		{
			Start(resolver, (Renderer renderer, Content content, InputCommands input) =>
			{
				var label = CreateLabel(content);
				var scene = new Scene();
				scene.Add(label);
				MemoryStream stream = scene.SaveToMemoryStream();
				CheckLabelsMatch(label, stream.CreateFromMemoryStream<Scene>().Find("Label") as Label);
				scene.Show(renderer, content, input);
			});
		}

		private static Label CreateLabel(Content content)
		{
			return new Label(content.Load<Image>("DeltaEngineLogo"), Centered)
			{
				Name = "Label",
				Color = Color.Red,
				Flip = FlipMode.Horizontal,
				IsVisible = true,
				RenderLayer = 5,
				Rotation = 45,
				RotationCenter = new Point(0.5f, 0.5f)
			};
		}

		private static readonly Rectangle Centered = new Rectangle(0.4f, 0.4f, 0.1f, 0.1f);

		private static void CheckLabelsMatch(Label label1, Label label2)
		{
			Assert.AreEqual(label1.Color, label2.Color);
			Assert.AreEqual(label1.DrawArea, label2.DrawArea);
			Assert.AreEqual(label1.Flip, label2.Flip);
			Assert.IsTrue(label2.ImageFilename == "dummy" || label2.ImageFilename == "DeltaEngineLogo");
			Assert.AreEqual(label1.IsVisible, label2.IsVisible);
			Assert.AreEqual(label1.RenderLayer, label2.RenderLayer);
			Assert.AreEqual(label1.Rotation, label2.Rotation);
			Assert.AreEqual(label1.RotationCenter, label2.RotationCenter);
		}
	}
}