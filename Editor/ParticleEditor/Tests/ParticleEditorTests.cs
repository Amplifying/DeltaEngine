using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
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
			Assert.AreEqual(500,particleEditor.MaxNumbersOfParticles);
			particleEditor.MaxNumbersOfParticles = 20;
			Assert.AreEqual(0, particleEditor.emitter.NumberOfActiveParticles);
		}
	}
}