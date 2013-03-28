using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BackgroundTests : TestStarter
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (Background bg) => Assert.IsTrue(bg.IsVisible));
		}

		[VisualTest]
		public void DrawSomethingOnTopOfTheBackground(Type type)
		{
			Start(type,
				(Background bg, Renderer renderer) =>
					renderer.Add(new Rect(Rectangle.FromCenter(Point.Half, Size.Half),
						Color.Yellow)));
		}
	}
}