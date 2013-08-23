using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	class FontPreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void LoadFontToDisplay()
		{
			fontPreviewer = new FontPreviewer();
			fontPreviewer.PreviewContent("Verdana12");
		}

		private FontPreviewer fontPreviewer;

		[Test]
		public void MoveFont()
		{
			var mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(0.5f, 0.5f), fontPreviewer.currentDisplayText.Center);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(2.5f, 2.5f), fontPreviewer.currentDisplayText.Center);	
		}

		[Test]
		public void SettingNewFontSetsDefaultPosition()
		{
			var mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(0.5f, 0.5f), fontPreviewer.currentDisplayText.Center);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(2.5f, 2.5f), fontPreviewer.currentDisplayText.Center);
			fontPreviewer.PreviewContent("DeltaEngineLogo");
			Assert.AreEqual(new Point(0.5f, 0.5f), fontPreviewer.currentDisplayText.Center);
		}
	}
}
