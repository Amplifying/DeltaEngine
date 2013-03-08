using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Devices;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	public class TestInputResolver : TestModuleResolver
	{
		public TestInputResolver(TestResolver testResolver) 
			: base(testResolver)
		{
			AllocateStates();
			InitializePositions();
			SetupInput();
		}

		public State[] KeyboardStates { get; set; }
		public State[] MouseButtonStates { get; set; }
		public State[] TouchStates { get; set; }
		public State[] GamePadButtonStates { get; set; }
		public Point CurrentMousePosition { get; set; }
		public Point CurrentTouchPosition { get; set; }

		private const int MaxNumberOfTouchIndices = 10;

		private void AllocateStates()
		{
			KeyboardStates = new State[(int)Key.NumberOfKeys];
			MouseButtonStates = new State[MouseButton.Left.GetCount()];
			TouchStates = new State[MaxNumberOfTouchIndices];
			GamePadButtonStates = new State[GamePadButton.A.GetCount()];
		}

		private void InitializePositions()
		{
			CurrentMousePosition = Point.Half;
			CurrentTouchPosition = Point.Half;
		}

		private void SetupInput()
		{
			SetupMockKeyboard();
			SetupMockMouse();
			SetupTouch();
			SetupGamePad();
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
			var mouse = testResolver.RegisterMock<Mouse>();
			mouse.SetupGet(k => k.IsAvailable).Returns(true);
			mouse.Setup(k => k.SetPosition(It.IsAny<Point>())).Callback(
				(Point p) => CurrentMousePosition = p);
			mouse.SetupGet(k => k.Position).Returns(() => CurrentMousePosition);
			mouse.SetupGet(k => k.ScrollWheelValue).Returns(0);
			SetupMockMouseButtons(mouse);
		}

		private void SetupMockMouseButtons(Mock<Mouse> mouse)
		{
			mouse.SetupGet(k => k.LeftButton).Returns(() => MouseButtonStates[(int)MouseButton.Left]);
			mouse.SetupGet(k => k.MiddleButton).Returns(() => MouseButtonStates[(int)MouseButton.Middle]);
			mouse.SetupGet(k => k.RightButton).Returns(() => MouseButtonStates[(int)MouseButton.Right]);
			mouse.SetupGet(k => k.X1Button).Returns(() => MouseButtonStates[(int)MouseButton.X1]);
			mouse.SetupGet(k => k.X2Button).Returns(() => MouseButtonStates[(int)MouseButton.X2]);
			mouse.Setup(k => k.GetButtonState(It.IsAny<MouseButton>())).Returns(
				(MouseButton button) => MouseButtonStates[(int)button]);
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
