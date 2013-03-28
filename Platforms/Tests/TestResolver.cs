using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using Moq;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Special resolver for unit tests that mocks all the integration classes (Window, Device, etc.)
	/// </summary>
	public class TestResolver : IDisposable
	{
		public TestResolver()
		{
			new TestCoreResolver(this).Register();
			testRenderingResolver = new TestRenderingResolver(this);
			testRenderingResolver.Register();
			testInputResolver = new TestInputResolver(this);
			testInputResolver.Register();
			testMultimediaResolver = new TestMultimediaResolver(this, testRenderingResolver);
			testMultimediaResolver.Register();
			new TestLoggingResolver(this).Register();
			new TestPlatformsResolver(this).Register();
			new TestContentResolver(this, testRenderingResolver.MockImage.Object).Register();
		}

		private readonly TestRenderingResolver testRenderingResolver;
		private readonly TestInputResolver testInputResolver;
		private readonly TestMultimediaResolver testMultimediaResolver;
		public int NumberOfVerticesDrawn { get; set; }

		public void AdvanceTimeAndExecuteRunners(float timeToAddInSeconds)
		{
			testStart.AdvanceTimeAndExecuteRunners(timeToAddInSeconds);
		}

		private TestAutofacStarter testStart = new TestAutofacStarter();

		public void SetTestStarter(AutofacStarter starter)
		{
			testStart = starter as TestAutofacStarter;
		}

		public bool IsMusicStopCalled()
		{
			return testMultimediaResolver.MusicStopCalled;
		}

		public Mock<T> RegisterMock<T>() where T : class
		{
			return testStart.RegisterMock<T>();
		}

		public void SetKeyboardState(Key key, State state)
		{
			testInputResolver.KeyboardStates[(int)key] = state;
		}

		public T RegisterMock<T>(T instance) where T : class
		{
			return testStart.RegisterMock(instance);
		}

		public void SetMouseButtonState(MouseButton button, State state)
		{
			testInputResolver.SetMouseButtonState(button, state);
		}

		public void SetMousePosition(Point newMousePosition)
		{
			testInputResolver.SetMousePosition(newMousePosition);
		}

		public void SetTouchState(int touchIndex, State state, Point newTouchPosition)
		{
			testInputResolver.CurrentTouchPosition = newTouchPosition;
			testInputResolver.TouchStates[touchIndex] = state;
		}

		public void SetGamePadState(GamePadButton button, State state)
		{
			testInputResolver.GamePadButtonStates[(int)button] = state;
		}

		public static implicit operator AutofacStarter(TestResolver resolver)
		{
			return resolver.testStart;
		}

		public void Dispose()
		{
			testStart.Dispose();
		}

		public void Start<AppEntryRunner>(int instancesToCreate = 1)
		{
			testStart.Start<AppEntryRunner>(instancesToCreate);
		}

		public BaseType Resolve<BaseType>(object customParameter = null)
		{
			return testStart.Resolve<BaseType>(customParameter);
		}
		
		public void Register<T>()
		{
			testStart.Register<T>();
		}

		public void RegisterSingleton<T>()
		{
			testStart.RegisterSingleton<T>();
		}

		public void RegisterAllUnknownTypesAutomatically()
		{
			testStart.RegisterAllUnknownTypesAutomatically();
		}

		public virtual void Run(Action runCode = null)
		{
			testStart.Run(runCode);
		}
	}
}