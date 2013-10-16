using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	internal class ParticlePreviewerTests : TestWithMocksOrVisually
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
			Assert.AreEqual(new Vector3D(0.5f, 0.5f, 0),
				particlePreviewer.currentDisplayParticle2D.Position);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector3D(0.5f, 0.5f, 0),
				particlePreviewer.currentDisplayParticle2D.Position);
		}

		[Test]
		public void ZoomCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			mockMouse.SetPosition(new Vector2D(0f, 0f));
			Assert.AreEqual(0.1f,
				particlePreviewer.currentDisplayParticle2D.EmitterData.Size.Start.Width);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector3D(0.5f, 0.5f, 0),
				particlePreviewer.currentDisplayParticle2D.Position);
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetPosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			//TODO: Assert.AreEqual(1.5f, particlePreviewer.currentDisplayParticle.DrawArea.Width);
		}

		[Test]
		public void SettingNewImageCreatesNewSizeAndPosition()
		{
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Vector2D(1f, 1f));
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetPosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector3D(0.5f, 0.5f, 0),
				particlePreviewer.currentDisplayParticle2D.Position);
			particlePreviewer.PreviewContent("TestParticle");
			Assert.AreEqual(new Vector3D(0.5f, 0.5f, 0),
				particlePreviewer.currentDisplayParticle2D.Position);
		}

		[Test]
		public void CreateDifferent3DParticles()
		{
			particlePreviewer.PreviewContent("PointEmitter3D");
			particlePreviewer.PreviewContent("LineEmitter3D");
			particlePreviewer.PreviewContent("BoxEmitter3D");
			particlePreviewer.PreviewContent("SphericalEmitter3D");
		}
	}
}