using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class ImageAnimationPreviewerTests : TestWithMocksOrVisually
	{
		[Test]
		public void Setup()
		{
			imageAnimationPreviewer = new ImageAnimationPreviewer();
			imageAnimationPreviewer.PreviewContent("ImageAnimation");
		}

		private ImageAnimationPreviewer imageAnimationPreviewer;
	}
}