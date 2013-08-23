using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	internal class ImagePreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			imagePreviewer = new ImagePreviewer();
			imagePreviewer.PreviewContent("DeltaEngineLogo");
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
		}

		private ImagePreviewer imagePreviewer;
		private MockMouse mockMouse;

		[Test]
		public void MoveCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(0.5f, 0.5f), imagePreviewer.currentDisplaySprite.Center);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(2.5f, 2.5f), imagePreviewer.currentDisplaySprite.Center);
		}

		[Test]
		public void ZoomCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(0.5f, 0.5f), imagePreviewer.currentDisplaySprite.Center);
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(1.5f, imagePreviewer.currentDisplaySprite.DrawArea.Width);
		}

		[Test]
		public void SettingNewImageCreatesNewSizeAndPosition()
		{
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(2.5f, 2.5f), imagePreviewer.currentDisplaySprite.Center);
			Assert.AreEqual(1.5f, imagePreviewer.currentDisplaySprite.DrawArea.Width);
			imagePreviewer.PreviewContent("DeltaEngineLogo");
			Assert.AreEqual(new Point(0.5f, 0.5f), imagePreviewer.currentDisplaySprite.Center);
			Assert.AreEqual(0.5f, imagePreviewer.currentDisplaySprite.DrawArea.Width);
		}
	}
}