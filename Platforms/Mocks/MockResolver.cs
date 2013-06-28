using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Input;
using DeltaEngine.Logging;
using DeltaEngine.Physics2D.Farseer;
using DeltaEngine.Rendering.Cameras;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Platforms.Mocks
{
	/// <summary>
	/// Special resolver for unit tests to mocks all the integration classes (Window, Device, etc.)
	/// </summary>
	public class MockResolver : AutofacStarter
	{
		public MockResolver()
		{
			new MockContentLoader(new AutofacContentDataResolver(this));
			Time.Current = new MockTime();
			RegisterMock(Time.Current);
			Logger.Current = new MockLogger();
			RegisterMock(Logger.Current);
			Register<MockClient>();
			RegisterSingleton<MockSettings>();
			Window = RegisterMock(new MockWindow());
			RegisterSingleton<MockInAppPurchase>();
			var device = new MockDevice();
			RegisterMock(device);
			RegisterMock(new MockDrawing(device));
			Register<MockImage>();
			Register<MockMesh>();
			RegisterSingleton<QuadraticScreenSpace>();
			RegisterSingleton<MockScreenshotCapturer>();
			RegisterSingleton<LookAtCamera>();
			RegisterSingleton<MockSoundDevice>();
			Register<MockSound>();
			Register<MockMusic>();
			Register<MockVideo>();
			RegisterSingleton<MockKeyboard>();
			RegisterSingleton<MockMouse>();
			RegisterSingleton<MockTouch>();
			RegisterSingleton<MockGamePad>();
			RegisterSingleton<InputCommands>();
			RegisterSingleton<FarseerPhysics>();
			RegisterSingleton<MockSystemInformation>();
			RegisterSingleton<RelativeScreenSpace>();	
		}

		public Window Window { get; private set; }

		public T RegisterMock<T>(T instance) where T : class
		{
			Type instanceType = instance.GetType();
			foreach (object mock in registeredMocks.Where(mock => mock.GetType() == instanceType))
				throw new UnableToRegisterAlreadyRegisteredMockClass(instance, mock);

			registeredMocks.Add(instance);
			alreadyRegisteredTypes.AddRange(instanceType.GetInterfaces());
			RegisterInstance(instance);
			return instance;
		}

		internal class UnableToRegisterAlreadyRegisteredMockClass : Exception
		{
			public UnableToRegisterAlreadyRegisteredMockClass(object instance, object mock)
				: base("New instance: " + instance + ", already registered mock class: " + mock) {}
		}

		private readonly List<object> registeredMocks = new List<object>();
	}
}