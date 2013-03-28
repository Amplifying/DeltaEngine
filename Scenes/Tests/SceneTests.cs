using System;
using DeltaEngine.Core;
using DeltaEngine.Core.Xml;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	public class SceneTests : TestStarter
	{
		[VisualTest]
		public void Draw(Type resolver)
		{
			Start(resolver, (Content content, InputCommands input, Renderer renderer) =>
			{
				Scene scene = CreateScene(content);
				var label = new Label(content.Load<Image>("DeltaEngineLogo"), Centered);
				scene.Show(renderer, content, input);
				scene.Add(label);
				Assert.AreEqual(5, scene.Controls.Count);
				Assert.AreEqual(5, renderer.NumberOfActiveRenderableObjects);
			});
		}

		private static Scene CreateScene(Content content)
		{
			var image = content.Load<Image>("DeltaEngineLogo");
			var scene = new Scene();
			scene.Add(new Label(image, TopLeft) { Color = Color.Red, Rotation = 45.0f });
			scene.Add(new Label(image, TopRight) { Color = Color.Blue, Rotation = 90.0f });
			scene.Add(new Button(image, Bottom) { Color = Color.Red, Rotation = 180.0f });
			var vectorTextContent = content.Load<XmlContent>("TestXml");
			scene.Add(new VectorTextControl(vectorTextContent, Top, Height) { Text = "Hello" });
			return scene;
		}

		private static readonly Point Top = new Point(0.4f, 0.3f);
		private const float Height = 0.05f;
		private static readonly Rectangle TopLeft = new Rectangle(0.3f, 0.4f, 0.1f, 0.1f);
		private static readonly Rectangle TopRight = new Rectangle(0.6f, 0.4f, 0.1f, 0.1f);
		private static readonly Rectangle Bottom = new Rectangle(0.45f, 0.6f, 0.1f, 0.1f);
		private static readonly Rectangle Centered = new Rectangle(0.475f, 0.475f, 0.05f, 0.05f);

		[IntegrationTest]
		public void RemoveControl(Type resolver)
		{
			Start(resolver, (Content content, InputCommands input, Renderer renderer) =>
			{
				Scene scene = CreateScene(content);
				scene.Show(renderer, content, input);
				scene.Remove(scene.Controls[1]);
				Assert.AreEqual(3, scene.Controls.Count);
				Assert.AreEqual(3, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[IntegrationTest]
		public void Clear(Type resolver)
		{
			Start(resolver, (Content content, InputCommands input, Renderer renderer) =>
			{
				Scene scene = CreateScene(content);
				scene.Show(renderer, content, input);
				scene.Clear();
				Assert.AreEqual(0, scene.Controls.Count);
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[Test]
		public void Find()
		{
			var image = new TestResolver().Resolve<Content>().Load<Image>("DeltaEngineLogo");
			var scene = new Scene();
			var label1 = new Label(image, Rectangle.Zero) { Name = "Label1" };
			scene.Add(label1);
			var label2 = new Label(image, Rectangle.Zero) { Name = "Label2" };
			scene.Add(label2);
			var button = new Button(image, Rectangle.Zero) { Name = "Button" };
			scene.Add(button);
			Assert.AreEqual(label2, scene.Find("Label2"));
			Assert.AreEqual(button, scene.Find("Button"));
			Assert.AreEqual(null, scene.Find("unknown"));
		}

		[IntegrationTest]
		public void ShowAndHide(Type resolver)
		{
			Start(resolver, (Content content, InputCommands input, Renderer renderer) =>
			{
				Scene scene = CreateScene(content);
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
				scene.Hide();
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
				scene.Show(renderer, content, input);
				scene.Show(renderer, content, input);
				Assert.AreEqual(4, renderer.NumberOfActiveRenderableObjects);
				scene.Hide();
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[IntegrationTest]
		public void Dispose(Type resolver)
		{
			Start(resolver, (Content content, InputCommands input, Renderer renderer) =>
			{
				Scene scene = CreateScene(content);
				scene.Show(renderer, content, input);
				scene.Dispose();
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
			});
		}
	}
}