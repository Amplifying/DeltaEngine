using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace LogoApp.Tests
{
	public class BouncingLogoTests : TestWithMocksOrVisually
	{
		[Test]
		public void Create()
		{
			var logo = new BouncingLogo();
			Assert.IsTrue(logo.DrawArea.Center.X > 0);
			Assert.IsTrue(logo.DrawArea.Center.Y > 0);
			Assert.AreNotEqual(Color.Black, logo.Color);
		}

		[Test]
		public void RunAFewTimesAndCloseGame()
		{
			new BouncingLogo();
		}

		[Test]
		public void ShowManyLogos()
		{
			for (int i = 0; i < 100; i++)
				new BouncingLogo();
		}
	}
}