using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class TextButtonTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void AddText(Type resolver)
		{
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var background = content.Load<Image>("DefaultButtonBackground");
				var button = new TextButton(background, content) { Text = "Hello" };
				Assert.AreEqual("Hello", button.Text);
			});
		}

		[VisualTest]
		public void RenderMovingGrowingTextButton(Type resolver)
		{
			Button button = null;
			Start(resolver, (Scene s, ContentLoader content) =>
			{
				var background = content.Load<Image>("DefaultButtonBackground");
				button = new TextButton(background, content)
				{
					DrawArea = ScreenCenter,
					Text = "Hello",
					Color = Color.LightGray,
					NormalColor = Color.LightGray,
					MouseoverColor = Color.White,
					PressedColor = Color.Red
				};
			}, () =>
			{
				var center = button.DrawArea.Center + new Point(0.01f, 0.01f) * Time.Current.Delta;
				var size = button.DrawArea.Size * (1.0f + Time.Current.Delta / 10);
				button.DrawArea = Rectangle.FromCenter(center, size);
			});
		}

		private static readonly Rectangle ScreenCenter = Rectangle.FromCenter(0.5f, 0.5f, 0.3f, 0.1f);
	}
}