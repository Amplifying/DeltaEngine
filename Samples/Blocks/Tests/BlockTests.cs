using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Block
	/// </summary>
	public class BlockTests : TestStarter
	{
		[Test]
		public void ConstructorTopLeft()
		{
			var resolver = new TestResolver();
			var block = new Block(resolver.Resolve<Content>(), new FixedRandom(), new Point(1, 2));
			Assert.AreEqual(1, block.Left);
			Assert.AreEqual(2, block.Top);
		}

		[Test]
		public void Left()
		{
			var resolver = new TestResolver();
			var shape = new Block(resolver.Resolve<Content>(), new FixedRandom(), Point.Zero)
			{
				Left = 1
			};
			Assert.AreEqual(1, shape.Left);
			Assert.AreEqual(1, shape.Bricks[0].TopLeft.X);
			Assert.AreEqual(1, shape.Bricks[1].TopLeft.X);
			Assert.AreEqual(1, shape.Bricks[2].TopLeft.X);
			Assert.AreEqual(1, shape.Bricks[3].TopLeft.X);
		}

		[Test]
		public void Top()
		{
			var resolver = new TestResolver();
			var shape = new Block(resolver.Resolve<Content>(), new FixedRandom(), Point.Zero)
			{
				Top = 1
			};
			Assert.AreEqual(1, shape.Top);
			Assert.AreEqual(1, shape.Bricks[0].TopLeft.Y);
			Assert.AreEqual(1, shape.Bricks[1].TopLeft.Y);
			Assert.AreEqual(1, shape.Bricks[2].TopLeft.Y);
			Assert.AreEqual(1, shape.Bricks[3].TopLeft.Y);
		}

		[Test]
		public void RunMovesTheBlock()
		{
			Start(typeof(TestResolver), (Time time, Content content) =>
			{
				var block = new Block(content, new FixedRandom(), Point.Zero);
				testResolver.AdvanceTimeAndExecuteRunners(0.0167f);
				block.Run(time, 2.0f);
				Assert.AreEqual(0.0333f, block.Top, 0.001f);
			});
		}

		[VisualTest]
		public void RenderJBlock(Type resolver)
		{
			Start(resolver,
				(Renderer renderer, Content content) =>
					renderer.Add(new Block(content,
						new FixedRandom(new[] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.9f }), Point.Zero)));
		}
	}
}