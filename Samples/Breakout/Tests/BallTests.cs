using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BallTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (Paddle paddle, EmptyLevel level, TestBall ball) => {});
		}

		[Test]
		public void FireBall()
		{
			Start(typeof(MockResolver), (Ball ball) =>
			{
				Assert.IsTrue(ball.Visibility == Visibility.Show);
				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				var initialBallPosition = new Point(0.5f, 0.86f);
				Assert.AreEqual(initialBallPosition, ball.Position);
				mockResolver.input.SetKeyboardState(Key.Space, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				Assert.AreNotEqual(initialBallPosition, ball.Position);
			});
		}

		[Test]
		public void ReflectBall()
		{
			Start(typeof(MockResolver), (Ball ball) =>
			{
				mockResolver.input.SetKeyboardState(Key.Space, State.Pressing);
				mockResolver.input.SetKeyboardState(Key.CursorRight, State.Pressing);
				ball.DrawArea = Rectangle.FromCenter(new Point(0.1f, 0.2f),ball.Size);
				mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				Assert.AreNotEqual(0.5f, ball.Position.X);
			});
		}

		[VisualTest]
		public void BallShouldFollowPaddle(Type type)
		{
			Start(type, (Ball ball, Paddle paddle) =>
			{
				Assert.AreEqual(0.5f, ball.Position.X);
				Assert.AreEqual(ball.Position.X, paddle.Position.X);
				if (mockResolver != null)
				{
					mockResolver.input.SetKeyboardState(Key.CursorLeft, State.Pressed);
					mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
				}

				Assert.AreNotEqual(0.5f, ball.Position.X);
				Assert.AreEqual(ball.Position.X, paddle.Position.X);
			});
		}

		[Test]
		public void BounceOnRightSideToMoveLeft()
		{
			Start(typeof(MockResolver), (Paddle paddle, TestBall ball) =>
			{
				ball.CurrentVelocity = new Point(0.5f, 0f);
				Assert.AreEqual(new Point(0.5f, 0f), ball.CurrentVelocity);
				ball.SetPosition(new Point(1, 0.5f));
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.AreEqual(new Point(-0.5f, 0f), ball.CurrentVelocity);
			});
		}

		[Test]
		public void BallSize()
		{
			Assert.AreEqual(new Size(0.04f), Ball.BallSize);
		}

		[Test]
		public void BounceOnLeftSideToMoveRight()
		{
			Start(typeof(MockResolver), (Paddle paddle, TestBall ball) =>
			{
				ball.CurrentVelocity = new Point(0.5f, 0.1f);
				ball.SetPosition(new Point(0, 0.5f));
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.AreEqual(new Point(0.5f, 0.1f), ball.CurrentVelocity);
			});
		}

		[Test]
		public void BounceOnTopSideToMoveDown()
		{
			Start(typeof(MockResolver), (Paddle paddle, TestBall ball) =>
			{
				ball.CurrentVelocity = new Point(-0.5f, -0.5f);
				ball.SetPosition(new Point(0.5f, 0));
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.AreEqual(new Point(-0.5f, 0.5f), ball.CurrentVelocity);
			});
		}

		[Test]
		public void BounceOnBottomSideToLoseBall()
		{
			Start(typeof(MockResolver), (Paddle paddle, TestBall ball) =>
			{
				ball.CurrentVelocity = new Point(-0.5f, -0.5f);
				ball.SetPosition(new Point(0.5f, 1.0f));
				Assert.IsFalse(ball.IsCurrentlyOnPaddle);
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.IsTrue(ball.IsCurrentlyOnPaddle);
				Assert.AreEqual(Point.Zero, ball.CurrentVelocity);
			});
		}

		[Test]
		public void PaddleCollision()
		{
			Start(typeof(MockResolver), (TestBall ball, Paddle paddle) =>
			{
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				ball.CurrentVelocity = new Point(0f, 0.1f);
				ball.SetPosition(paddle.Position);
				Assert.IsFalse(ball.IsCurrentlyOnPaddle);
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.AreEqual(new Point(0f, -0.1015f), ball.CurrentVelocity);
			});
		}

		[Test]
		public void ReleaseBallTwice()
		{
			Start(typeof(MockResolver), (Paddle paddle, TestBall ball) =>
			{
				mockResolver.input.SetKeyboardState(Key.Space, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Point velocity = ball.CurrentVelocity;
				mockResolver.input.SetKeyboardState(Key.Space, State.Pressing);
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.AreEqual(velocity, ball.CurrentVelocity);
			});
		}

		[Test]
		public void GetDrawPosition()
		{
			Start(typeof(MockResolver), (Paddle paddle, TestBall ball) =>
			{
				mockResolver.AdvanceTimeAndExecuteRunners(0.01f);
				Assert.AreEqual(new Rectangle(0.48f, 0.84f, 0.04f, 0.04f), ball.DrawArea);
			});
		}
	}
}