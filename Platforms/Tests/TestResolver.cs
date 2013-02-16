using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Input.Devices;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering;
using Moq;
using NUnit.Framework;
using Device = DeltaEngine.Graphics.Device;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Special resolver for unit tests that mocks all the integration classes (Window, Device, etc.)
	/// </summary>
	public class TestResolver : AutofacResolver
	{
		public TestResolver()
		{
			SetupCore();
			SetupWindow();
			SetupGraphics();
			SetupRenderer();
			SetupInput();
			SetupMultimedia();
			SetupPlatforms();
		}

		private void SetupCore()
		{
			var elapsedTimeMock = new Mock<ElapsedTime>();
			int ticks = 0;
			elapsedTimeMock.Setup(e => e.GetTicks()).Returns(() => ++ticks);
			elapsedTimeMock.SetupGet(e => e.TicksPerSecond).Returns(60);
			RegisterMock(elapsedTimeMock.Object);
			RegisterMock(new Mock<Time>(elapsedTimeMock.Object).Object);
			RegisterSingleton<Content>();
			RegisterSingleton<PseudoRandom>();
		}

		private Mock<T> RegisterMock<T>() where T : class
		{
			var mock = new Mock<T>();
			RegisterMock(mock.Object);
			return mock;
		}

		private T RegisterMock<T>(T instance) where T : class
		{
			Type instanceType = instance.GetType();
			Assert.IsFalse(registeredMocks.Any(mock => mock.GetType() == instanceType));
			registeredMocks.Add(instance);
			alreadyRegisteredTypes.AddRange(instanceType.GetInterfaces());
			RegisterInstance(instance);
			return instance;
		}

		private readonly List<object> registeredMocks = new List<object>();

		private void SetupWindow()
		{
			var windowMock = RegisterMock<Window>();
			windowMock.Setup(w => w.IsVisible).Returns(true);
			windowMock.Setup(w => w.IsClosing).Returns(true);
			windowMock.SetupProperty(w => w.Title, "WindowMock");
			var currentSize = new Size(800, 600);
			windowMock.SetupGet(w => w.TotalPixelSize).Returns(() => currentSize);
#pragma warning disable 0618
			windowMock.SetupSet(w => w.TotalPixelSize).Callback(s =>
			{
				currentSize = s;
				windowMock.Raise(w => w.ViewportSizeChanged += null, s);
			});
			windowMock.SetupGet(w => w.ViewportPixelSize).Returns(() => currentSize);
			window = windowMock.Object;
		}

		private Window window;

		private void SetupGraphics()
		{
			SetupGraphicsDevice();
			SetupDrawing();
			SetupImage();
		}

		private void SetupGraphicsDevice()
		{
			device = new Mock<Device>().Object;
			RegisterMock(device);
		}

		private void SetupDrawing()
		{
			var mockDrawing = new Mock<Drawing>(device);
			mockDrawing.Setup(
				d => d.DrawVertices(It.IsAny<VerticesMode>(), It.IsAny<VertexPositionColor[]>())).Callback(
					(VerticesMode mode, VertexPositionColor[] vertices) =>
						NumberOfVerticesDrawn += vertices.Length);
			drawing = RegisterMock(mockDrawing.Object);
		}

		private void SetupImage()
		{
			var mockImage = new Mock<Image>("dummy", drawing);
			mockImage.SetupGet(i => i.PixelSize).Returns(new Size(128, 128));
			mockImage.CallBase = true;
			RegisterMock(mockImage.Object);
		}

		private Device device;
		public int NumberOfVerticesDrawn { get; set; }
		private Drawing drawing;

		private void SetupRenderer()
		{
			screen = RegisterMock(new Mock<ScreenSpace>(window).Object);
			RegisterMock(new Mock<Renderer>(drawing, screen).Object);
		}

		private ScreenSpace screen;

		private void SetupInput()
		{
			SetupMockKeyboard();
			SetupMockMouse();
			SetupTouch();
			SetupGamePad();
			RegisterSingleton<Input.InputCommands>();
		}

		private void SetupMockKeyboard()
		{
			var keyboard = RegisterMock<Keyboard>();
			keyboard.SetupGet(k => k.IsAvailable).Returns(true);
			keyboard.Setup(k => k.GetKeyState(It.IsAny<Key>())).Returns(
				(Key key) => keyboardStates[(int)key]);
		}

		private readonly State[] keyboardStates = new State[(int)Key.NumberOfKeys];

		public void SetKeyboardState(Key key, State state)
		{
			keyboardStates[(int)key] = state;
		}

		private void SetupMockMouse()
		{
			var mouse = RegisterMock<Mouse>();
			mouse.SetupGet(k => k.IsAvailable).Returns(true);
			mouse.Setup(k => k.SetPosition(It.IsAny<Point>())).Callback(
				(Point p) => currentMousePosition = p);
			mouse.SetupGet(k => k.Position).Returns(() => currentMousePosition);
			mouse.SetupGet(k => k.ScrollWheelValue).Returns(0);
			mouse.SetupGet(k => k.LeftButton).Returns(() => mouseButtonStates[(int)MouseButton.Left]);
			mouse.SetupGet(k => k.MiddleButton).Returns(() => mouseButtonStates[(int)MouseButton.Middle]);
			mouse.SetupGet(k => k.RightButton).Returns(() => mouseButtonStates[(int)MouseButton.Right]);
			mouse.SetupGet(k => k.X1Button).Returns(() => mouseButtonStates[(int)MouseButton.X1]);
			mouse.SetupGet(k => k.X2Button).Returns(() => mouseButtonStates[(int)MouseButton.X2]);
			mouse.Setup(k => k.GetButtonState(It.IsAny<MouseButton>())).Returns(
				(MouseButton button) => mouseButtonStates[(int)button]);
		}

		private Point currentMousePosition = Point.Half;
		private readonly State[] mouseButtonStates = new State[MouseButton.Left.GetCount()];

		public void SetMouseButtonState(MouseButton button, State state, Point newMousePosition)
		{
			currentMousePosition = newMousePosition;
			mouseButtonStates[(int)button] = state;
		}

		private void SetupTouch()
		{
			var touch = RegisterMock<Touch>();
			touch.Setup(t => t.GetState(It.IsAny<int>())).Returns(
				(int touchIndex) => touchStates[touchIndex]);
			touch.Setup(t => t.GetPosition(It.IsAny<int>())).Returns(() => currentTouchPosition);
			touch.SetupGet(t => t.IsAvailable).Returns(true);
		}

		private Point currentTouchPosition = Point.Half;
		private readonly State[] touchStates = new State[MaxNumberOfTouchIndices];
		private const int MaxNumberOfTouchIndices = 10;

		public void SetTouchState(int touchIndex, State state, Point newTouchPosition)
		{
			currentTouchPosition = newTouchPosition;
			touchStates[touchIndex] = state;
		}

		private void SetupGamePad()
		{
			var touch = RegisterMock<GamePad>();
			touch.Setup(t => t.GetButtonState(It.IsAny<GamePadButton>())).Returns(
				(GamePadButton button) => gamePadButtonStates[(int)button]);
			touch.SetupGet(t => t.IsAvailable).Returns(true);
		}

		private readonly State[] gamePadButtonStates = new State[GamePadButton.A.GetCount()];

		public void SetGamePadState(GamePadButton button, State state)
		{
			gamePadButtonStates[(int)button] = state;
		}
		
		private void SetupMultimedia()
		{
			var soundDevice = RegisterMock<Multimedia.SoundDevice>();
			var mockSound = new Mock<Sound>("dummy", soundDevice.Object);
			mockSound.CallBase = true;
			mockSound.SetupGet(s => s.LengthInSeconds).Returns(0.48f);
			List<SoundInstance> playingSoundInstances = new List<SoundInstance>();
			mockSound.Setup(s => s.PlayInstance(It.IsAny<SoundInstance>())).Callback(
				(SoundInstance instance) => playingSoundInstances.Add(instance));
			mockSound.Setup(s => s.StopInstance(It.IsAny<SoundInstance>())).Callback(
				(SoundInstance instance) => playingSoundInstances.Remove(instance));
			mockSound.Setup(s => s.IsPlaying(It.IsAny<SoundInstance>())).Returns(
				(SoundInstance instance) => playingSoundInstances.Contains(instance));
				
			RegisterMock(mockSound.Object);
		}

		private void SetupPlatforms()
		{
			var autofac = RegisterMock<AutofacResolver>();
			autofac.CallBase = true;
		}

		protected override void MakeSureContainerIsInitialized()
		{
			if (IsAlreadyInitialized)
				return;

			base.MakeSureContainerIsInitialized();
			foreach (var instance in registeredMocks)
				RegisterInstanceAsRunnerOrPresenterIfPossible(instance);

			testElapsedMs = GetTimeInMsForSlowTests();
		}

		protected override void RegisterInstanceAsRunnerOrPresenterIfPossible(object instance)
		{
			var renderable = instance as Renderable;
			if (renderable != null)
				Resolve<Renderer>().Add(renderable);

			base.RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
		}

		private long testElapsedMs;

		private long GetTimeInMsForSlowTests()
		{
			if (testStarted != null)
				return testStarted.ElapsedMilliseconds;

			testStarted = new Stopwatch();
			testStarted.Start();
			return 0;
		}

		private Stopwatch testStarted;

		public void AdvanceTimeAndExecuteRunners(float timeToAddInSeconds)
		{
			int simulateRunTicks = (int)Math.Round(timeToAddInSeconds * 60);
			for (int tick = 0; tick < simulateRunTicks; tick++)
			{
				RunAllRunners();
				RunAllPresenters();
			}
		}

		public override void Dispose()
		{
			WarnIfUnitTestTakesTooLong();
			base.Dispose();
		}

		//ncrunch: no coverage start
		private void WarnIfUnitTestTakesTooLong()
		{
			if (StackTraceExtensions.ContainsUnitTest() && TookLongerThan10Ms())
				Debug.WriteLine("This unit test takes too long (" + testElapsedMs + "ms, max. 10ms is " +
					"allowed), please add Category(\"Slow\") to run it nightly instead!");
		}

		private bool TookLongerThan10Ms()
		{
			testElapsedMs = GetTimeInMsForSlowTests() - testElapsedMs;
			return testElapsedMs > 10;
		}
	}
}