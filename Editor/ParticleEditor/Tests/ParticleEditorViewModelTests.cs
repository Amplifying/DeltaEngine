using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D;
using NUnit.Framework;

namespace DeltaEngine.Editor.ParticleEditor.Tests
{
	internal class ParticleEditorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUpParticleEditor()
		{
			particleEditor = new ParticleEditorViewModel(new MockService("TestName", "TestProject"));
		}

		private ParticleEditorViewModel particleEditor;

		[Test]
		public void SetEmitterPosition()
		{
			var setPosition = new Vector3D(0.4f, 0.8f, 0.2f);
			particleEditor.EmitterPosition = setPosition;
			Assert.AreEqual(setPosition, particleEditor.EmitterPosition);
		}

		[Test]
		public void Set3DBillboardMode()
		{
			const BillboardMode SetMode = BillboardMode.UpAxis;
			particleEditor.SelectedBillBoardMode = SetMode.ToString();
			Assert.AreEqual(SetMode, particleEditor.EmitterData.BillboardMode);
			Assert.AreEqual(SetMode.ToString(), particleEditor.SelectedBillBoardMode);
		}

		[Test]
		public void SettingBillboardModeFromStringAlwaysGivesConsistentDefault()
		{
			particleEditor.SelectedBillBoardMode = "nonExistentMode";
			Assert.AreEqual(BillboardMode.CameraFacing.ToString(), particleEditor.SelectedBillBoardMode);
		}
	}
}