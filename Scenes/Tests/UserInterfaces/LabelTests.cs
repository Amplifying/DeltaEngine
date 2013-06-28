using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using DeltaEngine.Scenes.UserInterfaces;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.UserInterfaces
{
	public class LabelTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderGrowingLabel()
		{
			Label label = CreateLabel("Hello World");
			RunCode = () =>
			{
				var center = label.DrawArea.Center + new Point(0.01f, 0.01f) * Time.Current.Delta;
				var size = label.DrawArea.Size * (1.0f + Time.Current.Delta / 10);
				label.DrawArea = Rectangle.FromCenter(center, size);
			};
		}

		private static Label CreateLabel(string text)
		{
			var background = ContentLoader.Load<Image>("SimpleSubMenuBackground");
			var theme = new Theme
			{
				Label = new Theme.Appearance(background),
				Font = new Font("Verdana12")
			};
			var label = new Label(theme, Center) { Text = text };
			return label;
		}

		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.3f, 0.1f);

		[Test]
		public void SetText()
		{
			var label = CreateLabel("Hello");
			Assert.AreEqual("Hello", label.Text);
			label.Text = "World";
			Assert.AreEqual("World", label.Text);
			Window.CloseAfterFrame();
		}
	}
}