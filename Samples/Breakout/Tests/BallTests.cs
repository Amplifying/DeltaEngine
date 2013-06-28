using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class BallTests : TestWithMocksOrVisually
	{
		[Test]
		public void Draw(Type type)
		{
			Resolve<Paddle>();
			Resolve<EmptyLevel>();
			Resolve<TestBall>();
		}

		[Test]
		public void FireBall()
		{
			var ball = Resolve<Ball>();
			Assert.IsTrue(ball.Visibility == Visibility.Show);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			var initialBallPosition = new Point(0.5f, 0.86f);
			Assert.AreEqual(initialBallPosition, ball.Position);
			Resolve<MockKeyboard>().SetKeyboardState(Key.Space, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreNotEqual(initialBallPosition, ball.Position);
		}

		[Test]
		public void ReflectBall()
		{
			var ball = Resolve<Ball>();
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.Space, State.Pressing);
			keyboard.SetKeyboardState(Key.CursorRight, State.Pressing);
			ball.DrawArea = Rectangle.FromCenter(new Point(0.1f, 0.2f), ball.Size);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreNotEqual(0.5f, ball.Position.X);
		}

		[Test]
		public void BallShouldFollowPaddle(Type type)
		{
			var ball = Resolve<Ball>();
			var paddle = Resolve<Paddle>();
			Assert.AreEqual(0.5f, ball.Position.X);
			Assert.AreEqual(ball.Position.X, paddle.Position.X);
			Resolve<MockKeyboard>().SetKeyboardState(Key.CursorLeft, State.Pressed);
			resolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.AreNotEqual(0.5f, ball.Position.X);
			Assert.AreEqual(ball.Position.X, paddle.Position.X);
		}

		[Test]
		public void BounceOnRightSideToMoveLeft()
		{
			var ball = Resolve<TestBall>();
			Resolve<Paddle>();
			ball.CurrentVelocity = new Point(0.5f, 0f);
			Assert.AreEqual(new Point(0.5f, 0f), ball.CurrentVelocity);
			ball.SetPosition(new Point(1, 0.5f));
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Assert.AreEqual(new Point(-0.5f, 0f), ball.CurrentVelocity);
		}

		[Test]
		public void BallSize()
		{
			Assert.AreEqual(new Size(0.04f), Ball.BallSize);
		}

		[Test]
		public void BounceOnLeftSideToMoveRight()
		{
			var paddle = Resolve<Paddle>();
			var ball = new TestBall(paddle, Resolve<InputCommands>());
			
			ball.CurrentVelocity = new Point(0.5f, 0.1f);
			ball.SetPosition(new Point(0, 0.5f));
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Assert.AreEqual(new Point(0.5f, 0.1f), ball.CurrentVelocity);
		}

		[Test]
		public void BounceOnTopSideToMoveDown()
		{
			var ball = Resolve<TestBall>();
			Resolve<Paddle>();
			ball.CurrentVelocity = new Point(-0.5f, -0.5f);
			ball.SetPosition(new Point(0.5f, 0));
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Assert.AreEqual(new Point(-0.5f, 0.5f), ball.CurrentVelocity);
		}

		[Test]
		public void BounceOnBottomSideToLoseBall()
		{
			var ball = Resolve<TestBall>();
			Resolve<Paddle>();
			ball.CurrentVelocity = new Point(-0.5f, -0.5f);
			ball.SetPosition(new Point(0.5f, 1.0f));
			Assert.IsFalse(ball.IsCurrentlyOnPaddle);
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Assert.IsTrue(ball.IsCurrentlyOnPaddle);
			Assert.AreEqual(Point.Zero, ball.CurrentVelocity);
		}

		[Test]
		public void PaddleCollision()
		{
			var ball = Resolve<TestBall>();
			var paddle = Resolve<Paddle>();
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			ball.CurrentVelocity = new Point(0f, 0.1f);
			ball.SetPosition(paddle.Position);
			Assert.IsFalse(ball.IsCurrentlyOnPaddle);
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Assert.AreEqual(new Point(0f, -0.1015f), ball.CurrentVelocity);
		}

		[Test]
		public void ReleaseBallTwice()
		{
			var ball = Resolve<TestBall>();
			Resolve<Paddle>();
			Resolve<MockKeyboard>().SetKeyboardState(Key.Space, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Point velocity = ball.CurrentVelocity;
			Resolve<MockKeyboard>().SetKeyboardState(Key.Space, State.Pressing);
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Assert.AreEqual(velocity, ball.CurrentVelocity);
		}

		[Test]
		public void GetDrawPosition()
		{
			var ball = Resolve<TestBall>();
			Resolve<Paddle>();
			resolver.AdvanceTimeAndExecuteRunners(0.01f);
			Assert.AreEqual(new Rectangle(0.48f, 0.84f, 0.04f, 0.04f), ball.DrawArea);
		}
	}
}