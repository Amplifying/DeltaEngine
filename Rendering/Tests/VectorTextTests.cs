using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Rendering.Tests
{
	/// <summary>
	/// Visual tests for VectorText works
	/// </summary>
	public class VectorTextTests : TestStarter
	{
		[VisualTest]
		public void DrawSampleText(Type resolver)
		{
			Start(resolver, (Renderer renderer) =>
			{
				renderer.Add(new VectorText("Blue0123456789", new Point(0.1f, 0.45f), 0.05f) { Color = Color.Blue });
				renderer.Add(new VectorText("The Quick Brown Fox...", new Point(0.1f, 0.5f), 0.05f));
				renderer.Add(new VectorText("Jumps Over The Lazy Dog", new Point(0.1f, 0.55f), 0.05f));
			});
		}
	}
}