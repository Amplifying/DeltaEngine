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
		public void AlignTextCenter(Type resolver)
		{
			Start(resolver,
				(ContentLoader content) =>
				{
					new FontText(new Font(content, "Tahoma30"), "Text\ncenter\naligned", Point.Half,
						HorizontalAlignment.Centered);
				});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void AlignTextLeft(Type resolver)
		{
			Start(resolver,
				(ContentLoader content) =>
				{
					new FontText(new Font(content, "Tahoma30"), "Text\nleft\naligned", Point.Half,
						HorizontalAlignment.Left);
				});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void AlignTextRight(Type resolver)
		{
			Start(resolver,
				(ContentLoader content) =>
				{
					new FontText(new Font(content, "Tahoma30"), "Text\nright\naligned", Point.Half,
						HorizontalAlignment.Right);
				});
		}

		[VisualTest, ApproveFirstFrameScreenshot]
		public void LineSpacingMultiplier(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var font = new Font(content, "Tahoma30");
				var halfSpaced = new FontText(font, "Half\nline\nspacing", new Point(0.2f, 0.5f));
				halfSpaced.SetLineSpacing(0.5f);
				new FontText(font, "Full\nline\nspacing", new Point(0.5f, 0.5f));
				var doubleSpaced = new FontText(font, "Double\nline\nspacing", new Point(0.8f, 0.5f));
				doubleSpaced.SetLineSpacing(2.0f);
			});
		}
	}
}