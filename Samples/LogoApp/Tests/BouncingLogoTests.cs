using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace LogoApp.Tests
{
	public class BouncingLogoTests : TestStarter
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
				if (testResolver != null)
					testResolver.AdvanceTimeAndExecuteRunners(1);
			});
			if (remLogo != null)
				Assert.AreNotEqual(remPosition, remLogo.DrawArea.Center);
		}

		[Test]
		public void RunAFewTimesAndCloseGame()
		{
			var resolver = new TestResolver();
			resolver.Resolve<BouncingLogo>();
			resolver.AdvanceTimeAndExecuteRunners(5.0f);
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

		[VisualTest]
		public void ShowOneLogoWithFriction(Type resolver)
		{
			Start(resolver, (BouncingLogoWithFriction logo, InputCommands input) =>
			{
				if (testResolver != null)
					testResolver.AdvanceTimeAndExecuteRunners(30.0f);
			});
		}

		[VisualTest]
		public void ShowManyLogosWithFriction(Type resolver)
		{
			Start<BouncingLogoWithFriction>(resolver, 100);
		}

		[VisualTest]
		public void ShowOneLogoWithGravity(Type resolver)
		{
			Start(resolver, (BouncingLogoWithGravity logo) =>
			{
				if (testResolver != null)
				{
					testResolver.SetKeyboardState(Key.Space, State.Releasing);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
				}
			});
		}

		[VisualTest]
		public void ShowManyLogosWithGravity(Type resolver)
		{
			Start<BouncingLogoWithGravity>(resolver, 100);
		}

		[VisualTest]
		public void Show50LogosAndDisplayFps(Type resolver)
		{
			Start(resolver, (BouncingLogo initializeLogo, Resolver r) =>
			{
				for (int num = 0; num < 50; num++)
					r.Resolve<BouncingLogo>();
			}, (Window window, Time time) =>
			{
				if (time.CheckEvery(1) || resolver == typeof(TestResolver))
					window.Title = "Show50LogosAndDisplayFps: " + time.Fps;
			});
		}
	}
}