﻿using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	public class TestInputResolver : TestModuleResolver
	{
		public TestInputResolver(TestResolver testResolver)
			: base(testResolver) {}

		public override void Register()
		{
			AllocateStates();
			CurrentTouchPosition = Point.Half;
			SetupInput();			
		}

		internal Point CurrentTouchPosition { get; set; }

		private void AllocateStates()
		{
			KeyboardStates = new State[(int)Key.NumberOfKeys];
			TouchStates = new State[MaxNumberOfTouchIndices];
			GamePadButtonStates = new State[GamePadButton.A.GetCount()];
		}

		internal State[] KeyboardStates { get; set; }
		internal State[] TouchStates { get; set; }
		internal State[] GamePadButtonStates { get; set; }
		private const int MaxNumberOfTouchIndices = 10;

		private void SetupInput()
		{
			SetupMockKeyboard();
			SetupMockMouse();
			SetupTouch();
			SetupGamePad();
			testResolver.RegisterSingleton<PointerDevices>();
			testResolver.RegisterSingleton<InputCommands>();
		}

		private void SetupMockKeyboard()
		{
			var keyboard = testResolver.RegisterMock<Keyboard>();
			keyboard.SetupGet(k => k.IsAvailable).Returns(true);
			keyboard.Setup(k => k.GetKeyState(It.IsAny<Key>())).Returns(
				(Key key) => KeyboardStates[(int)key]);
		}

		private void SetupMockMouse()
		{
			mouse = testResolver.RegisterMock(new MockMouse());
		}

		private MockMouse mouse;

		internal void SetMouseButtonState(MouseButton button, State state)
		{
			mouse.SetButtonState(button, state);
		}

		internal void SetMousePosition(Point newMousePosition)
		{
			mouse.SetPosition(newMousePosition);
		}

		/// <summary>
		/// Unlike Keyboard, Touch and Gamepad, which are just interfaces, in the case 
		/// of the Mouse we need a specific mock class to be able to define a customized 
		/// behaviour that allows us to set the position and the mouse buttons states 
		/// in order to simulate events (mouse movement, button pressing) from the tests
		/// </summary>
		private class MockMouse : Mouse
		{
			public MockMouse()
			{
				Position = Point.Half;
			}

			public override bool IsAvailable
			{
				get { return true; }
			}

			public void SetButtonState(MouseButton button, State state)
			{
				if (button == MouseButton.Right)
					RightButton = state;
				else if (button == MouseButton.Middle)
					MiddleButton = state;
				else if (button == MouseButton.X1)
					X1Button = state;
				else if (button == MouseButton.X2)
					X2Button = state;
				else
					LeftButton = state;
			}

			public override void SetPosition(Point newPosition)
			{
				Position = newPosition;
			}

			public override void Run() {}

			public override void Dispose() {}
		}

		private void SetupTouch()
		{
			var touch = testResolver.RegisterMock<Touch>();
			touch.Setup(t => t.GetState(It.IsAny<int>())).Returns(
				(int touchIndex) => TouchStates[touchIndex]);
			touch.Setup(t => t.GetPosition(It.IsAny<int>())).Returns(() => CurrentTouchPosition);
			touch.SetupGet(t => t.IsAvailable).Returns(true);
		}

		private void SetupGamePad()
		{
			var touch = testResolver.RegisterMock<GamePad>();
			touch.Setup(t => t.GetButtonState(It.IsAny<GamePadButton>())).Returns(
				(GamePadButton button) => GamePadButtonStates[(int)button]);
			touch.SetupGet(t => t.IsAvailable).Returns(true);
		}
	}
}