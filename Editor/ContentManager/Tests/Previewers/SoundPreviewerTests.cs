using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class SoundPreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			soundPreviewer = new SoundPreviewer();
			soundPreviewer.PreviewContent("Sound");
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
		}

		private SoundPreviewer soundPreviewer;
		private MockMouse mockMouse;

		[Test]
		public void PlaySound()
		{
			soundPreviewer.sound.CreateSoundInstance();
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(1, soundPreviewer.sound.NumberOfInstances);
		}
	}
}
