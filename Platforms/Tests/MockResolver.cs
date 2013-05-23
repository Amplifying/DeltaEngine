using System;
using DeltaEngine.Entities;
using DeltaEngine.Platforms.Tests.ModuleMocks;

namespace DeltaEngine.Platforms.Tests
{
	/// <summary>
	/// Special resolver for unit tests to mocks all the integration classes (Window, Device, etc.)
	/// Like the App classes this class does not expose the underlying resolvers from other assembiles
	/// </summary>
	public sealed class MockResolver : IDisposable
	{
		public MockResolver()
		{
			new CoreMocks(resolver);
			rendering = new RenderingMocks(resolver);
			input = new InputMocks(resolver);
			multimedia = new MultimediaMocks(resolver);
			log = new LoggingMocks(resolver);
			new ContentMocks(resolver);
			new PhysicsMock(resolver);
		}

		internal readonly AutofacStarterForMockResolver resolver = new AutofacStarterForMockResolver();
		public readonly RenderingMocks rendering;
		public readonly InputMocks input;
		public readonly MultimediaMocks multimedia;
		public readonly LoggingMocks log;

		public EntitySystem EntitySystem
		{
			get { return resolver.Resolve<EntitySystem>(); }
		}

		public void Dispose()
		{
			resolver.Dispose();
		}

		public void AdvanceTimeAndExecuteRunners(float timeToAddInSeconds = 0.01666f)
		{
			resolver.AdvanceTimeAndExecuteRunners(timeToAddInSeconds);
		}
	}
}