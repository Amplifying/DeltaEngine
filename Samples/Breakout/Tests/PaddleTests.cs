using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace Breakout.Tests
{
	public class PaddleTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void Draw(Type type)
		{
			Start(type, (Paddle paddle, TestBall ball) =>
			{
				Assert.AreEqual(0.5f, paddle.Position.X);
				Assert.IsTrue(ball.IsCurrentlyOnPaddle);
				Assert.AreEqual(0.5f, ball.Position.X);
			});
		}

		[Test]
		public void ControlPaddleVirtuallyWithKeyboard()
		{
			Start(typeof(MockResolver), (Paddle paddle, TestBall ball) =>
			{
				mockResolver.input.SetKeyboardState(Key.CursorLeft, State.Pressed);
				AssertPaddleMovesLeftCorrectly(paddle);
				mockResolver.input.SetKeyboardState(Key.CursorLeft, State.Released);
				mockResolver.input.SetKeyboardState(Key.CursorRight, State.Pressed);
				AssertPaddleMovesRightCorrectly(paddle);
			});
		}

		private void AssertPaddleMovesLeftCorrectly(Paddle paddle)
		{
			Assert.AreEqual(0.5f, paddle.Position.X);
			mockResolver.AdvanceTimeAndExecuteRunners(0.1f);
			Assert.IsTrue(paddle.Position.X < 0.5f);
			Assert.IsTrue(paddle.Position.Y > 0.75f);
		}

		// ReSharper disable UnusedParameter.Local
		private void AssertPaddleMovesRightCorrectly(Paddle paddle)
		{
			mockResolver.AdvanceTimeAndExecuteRunners(0.2f);
			Assert.IsTrue(paddle.Position.X > 0.5f);
		}

		// ReSharper restore UnusedParameter.Local

		[Test]
		public void ControlPaddleVirtuallyWithGamePad()
		{
			Start(typeof(MockResolver), (Paddle paddle, TestBall ball) =>
			{
				mockResolver.input.SetGamePadState(GamePadButton.Left, State.Pressed);
				AssertPaddleMovesLeftCorrectly(paddle);
				mockResolver.input.SetGamePadState(GamePadButton.Left, State.Released);
				mockResolver.input.SetGamePadState(GamePadButton.Right, State.Pressed);
				AssertPaddleMovesRightCorrectly(paddle);
			});
		}

		[Test]
		public void ControlPaddleVirtuallyWithMouseAndTouch()
		{
			Start(typeof(MockResolver), (Paddle paddle, TestBall ball) =>
			{
				mockResolver.input.SetMousePosition(Point.Zero);
				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressed);
				AssertPaddleMovesLeftCorrectly(paddle);
				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Released);
				mockResolver.input.SetMousePosition(Point.One);
				mockResolver.input.SetMouseButtonState(MouseButton.Left, State.Pressed);
				AssertPaddleMovesRightCorrectly(paddle);
			});
		}

		[VisualTest]
		public void IsBallReleasedAfterSpacePressed(Type type)
		{
			TestBall remBall = null;
			Start(type, (Paddle paddle, TestBall ball) =>
			{
				remBall = ball;
				PressSpaceOneSecond();
			});
			AssertBallIsReleasedAndPaddleStay(remBall);
		}

		private void PressSpaceOneSecond()
		{
			if (mockResolver == null)
				return;

			mockResolver.input.SetKeyboardState(Key.Space, State.Pressing);
			mockResolver.AdvanceTimeAndExecuteRunners(1);
		}

		private static void AssertBallIsReleasedAndPaddleStay(TestBall remBall)
		{
			if (remBall != null)
			{
				Assert.IsFalse(remBall.IsCurrentlyOnPaddle);
				Assert.AreNotEqual(0.5f, remBall.Position.X);
			}
		}
	}
}