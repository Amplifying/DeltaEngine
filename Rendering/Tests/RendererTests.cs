using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests
{
	public class RendererTests
	{
		public RendererTests()
		{
			resolver = new TestResolver();
			renderer = resolver.Resolve<Renderer>();
		}

		protected readonly TestResolver resolver;
		protected readonly Renderer renderer;

		[SetUp]
		public void Init()
		{
			resolver.NumberOfVerticesDrawn = 0;
		}

		[SetUp]
		public void GotValidScreenSpace()
		{
			Assert.AreEqual(renderer.Screen, resolver.Resolve<ScreenSpace>());
		}

		[Test]
		public void Run()
		{
			var renderable = new TestRenderable();
			renderer.Add(renderable);
			renderer.Run(null);
			Assert.True(renderable.HasBeenRendered);
		}

		[Test]
		public void DrawLines()
		{
			renderer.DrawLine(Point.Zero, Point.One, Color.Red);
			Assert.AreEqual(2, resolver.NumberOfVerticesDrawn);
		}

		[Test]
		public void DrawRectangle()
		{
			renderer.DrawRectangle(Rectangle.Zero, Color.Yellow); 
			Assert.AreEqual(4, resolver.NumberOfVerticesDrawn);
		}

		private class TestRenderable : Renderable
		{
			protected override void Render(Renderer renderer, Time time)
			{
				HasBeenRendered = true;
			}

			public bool HasBeenRendered { get; private set; }
		}
	}
}