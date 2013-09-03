using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	class ParticlePreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			particlePreviewer = new ParticlePreviewer();
			particlePreviewer.PreviewContent("TestParticle");
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
		}

		private ParticlePreviewer particlePreviewer;
		private MockMouse mockMouse;

		[Test]
		public void MoveCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(0.5f, 0.5f), particlePreviewer.currentDisplayParticle.Center);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(2.5f, 2.5f), particlePreviewer.currentDisplayParticle.Center);
		}

		[Test]
		public void ZoomCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			mockMouse.SetPosition(new Point(0f, 0f));
			Assert.AreEqual(0.0f, particlePreviewer.currentDisplayParticle.DrawArea.Width);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(0.5f, 0.5f), particlePreviewer.currentDisplayParticle.Center);
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			//TODO: Assert.AreEqual(1.5f, particlePreviewer.currentDisplayParticle.DrawArea.Width);
		}

		[Test]
		public void SettingNewImageCreatesNewSizeAndPosition()
		{
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Point(2.5f, 2.5f), particlePreviewer.currentDisplayParticle.Center);
			particlePreviewer.PreviewContent("TestParticle");
			Assert.AreEqual(new Point(0.5f, 0.5f), particlePreviewer.currentDisplayParticle.Center);
		}
	}
}
