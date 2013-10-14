using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class ImageAnimationPreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			imageAnimationPreviewer = new ImageAnimationPreviewer();
			imageAnimationPreviewer.PreviewContent("DeltaEngineLogo");
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
		}

		private ImageAnimationPreviewer imageAnimationPreviewer;
		private MockMouse mockMouse;

		[Test]
		public void MoveCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f),
				imageAnimationPreviewer.currentDisplayAnimation.Center);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			mockMouse.SetPosition(new Vector2D(0.6f, 0.6f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.6f, 0.6f),
				imageAnimationPreviewer.currentDisplayAnimation.Center);
		}

		[Test]
		public void ZoomCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f),
				imageAnimationPreviewer.currentDisplayAnimation.Center);
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			mockMouse.SetPosition(new Vector2D(0.6f, 0.6f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(0.55f, imageAnimationPreviewer.currentDisplayAnimation.DrawArea.Width);
		}

		[Test]
		public void SettingNewImageCreatesNewSizeAndPosition()
		{
			mockMouse.SetButtonState(MouseButton.Left, State.Pressing);
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			mockMouse.SetPosition(new Vector2D(0.6f, 0.6f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.6f, 0.6f),
				imageAnimationPreviewer.currentDisplayAnimation.Center);
			Assert.AreEqual(0.55f, imageAnimationPreviewer.currentDisplayAnimation.DrawArea.Width);
			imageAnimationPreviewer.PreviewContent("DeltaEngineLogo");
			Assert.AreEqual(new Vector2D(0.5f, 0.5f),
				imageAnimationPreviewer.currentDisplayAnimation.Center);
			Assert.AreEqual(0.5f, imageAnimationPreviewer.currentDisplayAnimation.DrawArea.Width);
		}
	}
}