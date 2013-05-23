using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests
{
	public class SceneTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void RenderThreeLabelsAndAButton(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				Scene scene = CreateScene(content);
				var label = new Label(content.Load<Image>("DeltaEngineLogo"), Centered);
				scene.Show(entitySystem, content, input);
				scene.Add(label);
				entitySystem.Run();
				Assert.AreEqual(4, scene.Controls.Count);
				Assert.AreEqual(4,
					entitySystem.GetHandler<SortAndRenderEntity2D>().NumberOfActiveRenderableObjects);
			});
		}

		private static Scene CreateScene(ContentLoader content)
		{
			var image = content.Load<Image>("DeltaEngineLogo");
			var scene = new Scene();
			scene.Add(new Label(image, TopLeft) { Sprite = { Color = Color.Red, Rotation = 45.0f } });
			scene.Add(new Label(image, TopRight) { Sprite = { Color = Color.Blue, Rotation = 90.0f } });
			scene.Add(new Button(image, Bottom) { Sprite = { Color = Color.Red, Rotation = 180.0f } });
			//TODO: VectorText
			//var vectorTextContent = content.Load<XmlContent>("VectorText");
			//scene.Add(new VectorTextControl(vectorTextContent, Top, Height)
			//{
			//	VectorText = { Text = "Hello" }
			//});
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
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				Scene scene = CreateScene(content);
				scene.Show(entitySystem, content, input);
				scene.Remove(scene.Controls[1]);
				entitySystem.Run();
				Assert.AreEqual(2, scene.Controls.Count);
				Assert.AreEqual(3,
					entitySystem.GetHandler<SortAndRenderEntity2D>().NumberOfActiveRenderableObjects);
			});
		}

		[IntegrationTest]
		public void Clear(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				Scene scene = CreateScene(content);
				scene.Show(entitySystem, content, input);
				scene.Clear();
				Assert.AreEqual(0, scene.Controls.Count);
				Assert.AreEqual(0,
					entitySystem.GetHandler<SortAndRenderEntity2D>().NumberOfActiveRenderableObjects);
			});
		}

		[Test]
		public void Find()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
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
			});
		}

		[IntegrationTest]
		public void ShowAndHide(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				Scene scene = CreateScene(content);
				var renderer = entitySystem.GetHandler<SortAndRenderEntity2D>();
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
				scene.Hide();
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
				scene.Show(entitySystem, content, input);
				scene.Show(entitySystem, content, input);
				entitySystem.Run();
				Assert.AreEqual(3, renderer.NumberOfActiveRenderableObjects);
				scene.Hide();
				entitySystem.Run();
				Assert.AreEqual(0, renderer.NumberOfActiveRenderableObjects);
			});
		}

		[IntegrationTest]
		public void Dispose(Type resolver)
		{
			Start(resolver, (EntitySystem entitySystem, ContentLoader content, InputCommands input) =>
			{
				Scene scene = CreateScene(content);
				scene.Show(entitySystem, content, input);
				scene.Dispose();
				Assert.AreEqual(0,
					entitySystem.GetHandler<SortAndRenderEntity2D>().NumberOfActiveRenderableObjects);
			});
		}
	}
}