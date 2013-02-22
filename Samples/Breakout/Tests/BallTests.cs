using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BallTests : TestStarter
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (Paddle paddle, EmptyLevel level, TestBall ball) => { });
		}

		[VisualTest]
		public void DisposingShouldMakeTheBallDisappear(Type type)
		{
			Renderer remRenderer = null;
			Start(type, (Paddle paddle, TestBall ball, Renderer renderer) =>
			{
				paddle.Dispose();
				ball.Dispose();
				remRenderer = renderer;
			});
			if (remRenderer != null)
				Assert.AreEqual(0, remRenderer.NumberOfActiveRenderableObjects);
		}

		[Test]
		public void FireBall()
		{
			var resolver = new TestResolver();
			var ball = resolver.Resolve<Ball>();
			Assert.IsTrue(ball.IsVisible);
			resolver.Run();
			var initialBallPosition = new Point(0.5f, 0.76f);
			Assert.AreEqual(initialBallPosition, ball.Position);
			resolver.SetKeyboardState(Key.Space, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreNotEqual(initialBallPosition, ball.Position);
		}

		[Test]
		public void ReflectBall()
		{
			var resolver = new TestResolver();
			var ball = resolver.Resolve<Ball>();
			resolver.SetKeyboardState(Key.Space, State.Pressing);
			resolver.SetKeyboardState(Key.CursorRight, State.Pressing);
			ball.DrawArea.Center = new Point(0.1f, 0.2f);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreNotEqual(0.5f, ball.Position.X);
		}

		[VisualTest]
		public void BallShouldFollowPaddle(Type type)
		{
			Start(type, (Ball ball, Paddle paddle) =>
			{
				Assert.AreEqual(0.5f, ball.Position.X);
				Assert.AreEqual(ball.Position.X, paddle.Position.X);
				if (testResolver != null)
				{
					testResolver.SetKeyboardState(Key.CursorLeft, State.Pressed);
					testResolver.AdvanceTimeAndExecuteRunners(0.1f);
					Assert.AreNotEqual(0.5f, ball.Position.X);
					Assert.AreEqual(ball.Position.X, paddle.Position.X);
				}
			});
		}

		[Test]
		public void BounceOnRightSideToMoveLeft()
		{
			TestBall ball = CreateTestBall();
			ball.CurrentVelocity = new Point(0.5f, 0f);
			Assert.AreEqual(new Point(0.5f, 0f), ball.CurrentVelocity);
			ball.SetPosition(new Point(1, 0.5f));
			RunBallOneFrame();
			Assert.AreEqual(new Point(-0.5f, 0f), ball.CurrentVelocity);
		}

		private TestBall CreateTestBall()
		{
			ballResolver = new TestResolver();
			ballResolver.RegisterSingleton<Paddle>();
			return ballResolver.Resolve<TestBall>();
		}

		private TestResolver ballResolver = new TestResolver();

		private void RunBallOneFrame()
		{
			ballResolver.Resolve<Renderer>().Run(ballResolver.Resolve<Time>());
		}

		[Test]
		public void BounceOnLeftSideToMoveRight()
		{
			TestBall ball = CreateTestBall();
			ball.CurrentVelocity = new Point(0.5f, 0.1f);
			ball.SetPosition(new Point(0, 0.5f));
			RunBallOneFrame();
			Assert.AreEqual(new Point(0.5f, 0.1f), ball.CurrentVelocity);
		}

		[Test]
		public void BounceOnTopSideToMoveDown()
		{
			TestBall ball = CreateTestBall();
			ball.CurrentVelocity = new Point(-0.5f, -0.5f);
			ball.SetPosition(new Point(0.5f, 0));
			RunBallOneFrame();
			Assert.AreEqual(new Point(-0.5f, 0.5f), ball.CurrentVelocity);
		}

		[Test]
		public void BounceOnBottomSideToLoseBall()
		{
			TestBall ball = CreateTestBall();
			ball.CurrentVelocity = new Point(-0.5f, -0.5f);
			ball.SetPosition(new Point(0.5f, 1.0f));
			Assert.IsFalse(ball.IsCurrentlyOnPaddle);
			RunBallOneFrame();
			Assert.IsTrue(ball.IsCurrentlyOnPaddle);
			Assert.AreEqual(Point.Zero, ball.CurrentVelocity);
		}

		[Test]
		public void PaddleCollision()
		{
			TestBall ball = CreateTestBall();
			RunBallOneFrame();
			ball.CurrentVelocity = new Point(0f, 0.1f);
			var paddle = ballResolver.Resolve<Paddle>();
			ball.SetPosition(paddle.Position);
			Assert.IsFalse(ball.IsCurrentlyOnPaddle);
			RunBallOneFrame();
			Assert.AreEqual(new Point(0f, -0.1015f), ball.CurrentVelocity);
		}

		[Test]
		public void ReleaseBallTwice()
		{
			TestBall ball = CreateTestBall();
			ballResolver.SetKeyboardState(Key.Space, State.Pressing);
			RunBallOneFrame();
			Point velocity = ball.CurrentVelocity;
			ballResolver.SetKeyboardState(Key.Space, State.Pressing);
			RunBallOneFrame();
			Assert.AreEqual(velocity, ball.CurrentVelocity);
		}

		[Test]
		public void GetDrawPosition()
		{
			TestBall ball = CreateTestBall();
			RunBallOneFrame();
			Assert.AreEqual(Rectangle.FromCenter(0.5f, 0.76f, 0.04f, 0.04f), ball.DrawArea);
		}
	}
}