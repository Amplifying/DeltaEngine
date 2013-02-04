using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Windows;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class CursorPositionTranslaterTests : TestStarter
	{
		[IntegrationTest]
		public void GetClientPositionOnScreen(Type resolver)
		{
			Start(resolver, (CursorPositionTranslater translator, Window window, ScreenSpace screen) =>
			{
				var outsidePosition = screen.ToQuadraticSpace(new Point(-10, -10));
				var screenPos = translator.ToScreenPositionFromScreenSpace(outsidePosition);
				Assert.IsTrue(screenPos.X < window.PixelPosition.X || screenPos.Y < window.PixelPosition.Y);
				Assert.AreEqual(outsidePosition, translator.FromScreenPositionToScreenSpace(screenPos));
			});
		}

		[IntegrationTest]
		public void ConvertPixelFromScreenPositionAndBack(Type resolver)
		{
			Start(resolver, (CursorPositionTranslater positionTranslator, ScreenSpace screen) =>
			{
				var topLeftPixel = Point.Zero;
				var outside = positionTranslator.FromScreenPositionToScreenSpace(topLeftPixel);
				Assert.AreEqual(topLeftPixel, positionTranslator.ToScreenPositionFromScreenSpace(outside));
			});
		}
	}
}