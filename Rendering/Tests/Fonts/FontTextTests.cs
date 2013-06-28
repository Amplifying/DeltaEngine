using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Fonts
{
	internal class FontTextTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void TextShouldSayChangedText()
		{
			var text = new FontText(new Font("Tahoma30"), "To be changed", Point.Half);
			text.Text = "Changed\nText";
		}

		[Test, ApproveFirstFrameScreenshot]
		public void TextShouldBeInTheMiddleOfTheScreen()
		{
			var text = new FontText(new Font("Tahoma30"), "Middle", Point.Zero);
			text.SetPosition(Point.Half);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void CentralAlignedText()
		{
			new FontText(new Font("Tahoma30"), "Text\ncenter\naligned", center);
		}

		private readonly Point center = new Point(0.5f, 0.5f);
	}
}