using DeltaEngine.Datatypes;
using NUnit.Framework;
using DeltaEngine.Platforms;

namespace FountainApp.Tests
{
	public class FountainTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowOngoingFountain()
		{
			var fountain = new ParticleFountain(Point.Half);
			Assert.AreEqual(0.5f, fountain.Center.X);
		}

		[Test, CloseAfterFirstFrame]
		public void RunAFewTimesAndCloseGame()
		{
			var fountain = new ParticleFountain(Point.Half);
			AdvanceTimeAndUpdateEntities(0.2f);
			Assert.AreNotEqual(0, fountain.NumberOfActiveParticles);
		}
	}
}