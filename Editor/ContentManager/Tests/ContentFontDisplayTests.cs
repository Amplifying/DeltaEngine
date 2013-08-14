using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	class ContentFontDisplayTests : TestWithMocksOrVisually
	{
		[Test, Ignore]
		public void LoadFontToDisplay()
		{
			fontDisplay = new FontPreviewer();
			fontDisplay.PreviewContent("Gabriela48");
		}

		private FontPreviewer fontDisplay;
	}
}
