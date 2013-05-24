using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Input.Windows.Tests
{
	public class CursorPositionTranslaterTests : TestWithAllFrameworks
	{
		[IntegrationTest]
		public void GetClientPositionOnScreen(Type resolver)
		{
			Start(resolver, (CursorPositionTranslater translator, Window window, ScreenSpace screen) =>
			{
				var outsidePosition = screen.FromPixelSpace(new Point(-10, -10));
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