using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class LabelTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void SetText(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var label = CreateLabel(content, "Hello");
				Assert.AreEqual("Hello", label.Text);
				label.Text = "World";
				Assert.AreEqual("World", label.Text);
			});
		}

		private static Label CreateLabel(ContentLoader content, string text)
		{
			var background = content.Load<Image>("SimpleSubMenuBackground");
			var theme = new Theme
			{
				Label = new Theme.Appearance(background),
				Font = new Font(content, "Verdana12")
			};
			var label = new Label(theme, Center) { Text = text };
			return label;
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.3f, 0.1f);

		[VisualTest]
		public void RenderGrowingLabel(Type resolver)
		{
			Label label = null;
			Start(resolver,
				(Scene s, ContentLoader content) => { label = CreateLabel(content, "Hello World"); }, () =>
				{
					var center = label.DrawArea.Center + new Point(0.01f, 0.01f) * Time.Current.Delta;
					var size = label.DrawArea.Size * (1.0f + Time.Current.Delta / 10);
					label.DrawArea = Rectangle.FromCenter(center, size);
				});
		}
	}
}