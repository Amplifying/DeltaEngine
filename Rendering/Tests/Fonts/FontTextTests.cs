using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Fonts;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	internal class FontTextTests : TestWithAllFrameworks
	{
		[VisualTest, ApproveFirstFrameScreenshot]
		public void TextShouldSayChangedText(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var text = new FontText(new Font(content, "Tahoma30"), "To be changed", Point.Half);
				text.Text = "Changed\nText";
			});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void TextShouldBeInTheMiddleOfTheScreen(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var text = new FontText(new Font(content, "Tahoma30"), "Middle", Point.Zero);
				text.SetPosition(Point.Half);
			});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void CentralAlignedText(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var font = new Font(content, "Tahoma30");
				new FontText(font, "Text\ncenter\naligned", center);
			});
		}

		private readonly Point center = new Point(0.5f, 0.5f);
	}
}