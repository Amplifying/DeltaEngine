using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace LogoApp.Tests
{
	public class BouncingLogoTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void Create(Type resolver)
		{
			Start(resolver, (BouncingLogo logo) =>
			{
				Assert.IsTrue(logo.DrawArea.Center.X > 0);
				Assert.IsTrue(logo.DrawArea.Center.Y > 0);
				Assert.AreNotEqual(Color.Black, logo.Color);
			});
		}

		[IntegrationTest]
		public void RunOnce(Type resolver)
		{
			Point remPosition = Point.Zero;
			BouncingLogo remLogo = null;
			Start(resolver, (BouncingLogo logo) =>
			{
				remLogo = logo;
				remPosition = logo.DrawArea.Center;
			}, () =>
			{
				if (remLogo != null)
					Assert.AreNotEqual(remPosition, remLogo.DrawArea.Center);
			});
		}

		[Test]
		public void RunAFewTimesAndCloseGame()
		{
			Start(typeof(MockResolver),
				(BouncingLogo logo) => mockResolver.AdvanceTimeAndExecuteRunners(5.0f));
		}

		[VisualTest]
		public void ShowOneLogo(Type resolver)
		{
			Start<BouncingLogo>(resolver);
		}

		[VisualTest]
		public void ShowManyLogos(Type resolver)
		{
			Start<BouncingLogo>(resolver, 100);
		}
	}
}