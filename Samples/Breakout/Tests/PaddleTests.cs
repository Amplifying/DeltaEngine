using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Devices;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class PaddleTests : TestStarter
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (Paddle paddle) => { });
		}

		[Test]
		public void ControlPaddleVirtuallyWithKeyboard()
		{
			var resolver = new TestResolver();
			var paddle = resolver.Resolve<Paddle>();
			Assert.AreEqual(0.5f, paddle.Position.X);
			resolver.SetKeyboardState(Key.CursorLeft, State.Pressed);
			resolver.Run();
			resolver.SetKeyboardState(Key.CursorLeft, State.Released);
			Assert.IsTrue(paddle.Position.X < 0.5f);
			Assert.IsTrue(paddle.Position.Y > 0.75f);
			resolver.SetKeyboardState(Key.CursorRight, State.Pressed);
			resolver.Run();
			Assert.AreEqual(0.5f, paddle.Position.X);
		}

		[Test]
		public void ControlPaddleVirtuallyWithMouseAndTouch()
		{
			var resolver = new TestResolver();
			var paddle = resolver.Resolve<Paddle>();
			Assert.AreEqual(0.5f, paddle.Position.X);
			resolver.SetMouseButtonState(MouseButton.Left, State.Pressed, Point.One);
			resolver.Run();
			resolver.SetMouseButtonState(MouseButton.Left, State.Released, Point.One);
			Assert.IsTrue(paddle.Position.X > 0.5f);
			resolver.SetTouchState(0, State.Pressed, Point.Zero);
			resolver.Run();
			Assert.IsTrue(paddle.Position.X < 1.0f);
		}

		[Test]
		public void ControlPaddleVirtuallyWithGamePad()
		{
			var resolver = new TestResolver();
			var paddle = resolver.Resolve<Paddle>();
			Assert.AreEqual(0.5f, paddle.Position.X);
			resolver.SetGamePadState(GamePadButton.Left, State.Pressed);
			resolver.Run();
			resolver.SetGamePadState(GamePadButton.Left, State.Released);
			Assert.IsTrue(paddle.Position.X < 0.5f);
			resolver.SetGamePadState(GamePadButton.Right, State.Pressed);
			resolver.Run();
			Assert.AreEqual(0.5f, paddle.Position.X);
		}

		[VisualTest]
		public void IsBallReleasedAfterSpacePressed(Type type)
		{
			TestBall remBall = null;
			Start(type, (Paddle paddle, TestBall ball) =>
			{
				remBall = ball;
				Assert.AreEqual(0.5f, paddle.Position.X);
				Assert.IsTrue(ball.IsCurrentlyOnPaddle);
				Assert.AreEqual(0.5f, ball.Position.X);
				if (testResolver != null)
				{
					testResolver.SetKeyboardState(Key.Space, State.Pressing);
					testResolver.AdvanceTimeAndExecuteRunners(1);
				}
			});
			if (remBall != null)
			{
				Assert.IsFalse(remBall.IsCurrentlyOnPaddle);
				Assert.AreNotEqual(0.5f, remBall.Position.X);
			}
		}
	}
}