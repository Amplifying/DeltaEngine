using NUnit.Framework;
using DeltaEngine.Platforms;

namespace FountainApp.Tests
{
	public class FountainTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowOngoingFountain()
		{
				var fountain = new Fountain();
				Assert.AreEqual(0, fountain.FountainParticle.Center.X);
		}

		[Test]
		public void RunAFewTimesAndCloseGame()
		{
			var fountain = new Fountain();
			resolver.AdvanceTimeAndExecuteRunners(0.2f);
			Assert.AreNotEqual(0, fountain.FountainParticle.ParticlesCreated);
			Window.Dispose();
		}

	}
}