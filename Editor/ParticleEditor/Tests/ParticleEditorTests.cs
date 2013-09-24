using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Models;
using NUnit.Framework;

namespace DeltaEngine.Editor.ParticleEditor.Tests
{
	internal class ParticleEditorTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUpParticleEditor()
		{
			particleEditor = new ParticleEditorViewModel(new MockService("TestName", "TestProject"));
		}

		private ParticleEditorViewModel particleEditor;

		[Test]
		public void IfChangingMaxNumberOfParticlesThenRemoveAllParticles()
		{
			particleEditor.SelectedMaterial = "TestMaterial";
			particleEditor.ParticleName = "TestParticle";
			Assert.AreEqual(500, particleEditor.MaxNumbersOfParticles);
			particleEditor.MaxNumbersOfParticles = 20;
			Assert.AreEqual(0, particleEditor.emitter.NumberOfActiveParticles);
		}

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
			particleEditor.SelectedBillBoardMode = "nonExistantMode";
			Assert.AreEqual(BillboardMode.CameraFacing.ToString(), particleEditor.SelectedBillBoardMode);
		}

		[Test]
		public void SaveParticleEmitterContent() {}
	}
}