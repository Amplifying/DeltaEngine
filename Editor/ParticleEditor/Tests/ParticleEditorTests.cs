using DeltaEngine.Editor.Core;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ParticleEditor.Tests
{
	internal class ParticleEditorTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUpParticleEditor()
		{
			particleEditor = new ParticleEditorViewModel(Resolve<Service>());
		}

		private ParticleEditorViewModel particleEditor;

		[Test]
		public void IfChangingMaxNumberOfParticlesThenRemoveAllParticles()
		{
			Assert.AreEqual(500,particleEditor.MaxNumbersOfParticles);
			AdvanceTimeAndUpdateEntities(1f);
			Assert.GreaterOrEqual(particleEditor.emitter.NumberOfActiveParticles, 80);
			particleEditor.MaxNumbersOfParticles = 20;
			Assert.AreEqual(0, particleEditor.emitter.NumberOfActiveParticles);
		}
	}
}