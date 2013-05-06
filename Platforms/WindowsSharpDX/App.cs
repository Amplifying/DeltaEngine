using System;
using System.Collections.Generic;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Exchangable App entry point for Windows DirectX applications and visual tests.
	/// </summary>
	public class App
	{
		public void Start<AppEntryRunner>(int instancesToCreate = 1)
		{
			resolver.Start<AppEntryRunner>(instancesToCreate);
		}

		private readonly SharpDxResolver resolver = new SharpDxResolver();

		public void Start<AppEntryRunner, FirstClassToRegisterAndResolve>(int instancesToCreate = 1)
		{
			resolver.Start<AppEntryRunner, FirstClassToRegisterAndResolve>(instancesToCreate);
		}

		public void Start
			<AppEntryRunner, FirstClassToRegisterAndResolve, SecondClassToRegisterAndResolve>(
			int instancesToCreate = 1)
		{
			resolver.Start
				<AppEntryRunner, FirstClassToRegisterAndResolve, SecondClassToRegisterAndResolve>(
					instancesToCreate);
		}

		public void Start<AppEntryRunner>(List<Type> typesToRegisterAndResolve,
			int instancesToCreate = 1)
		{
			resolver.Start<AppEntryRunner>(typesToRegisterAndResolve, instancesToCreate);
		}

		public void RegisterSingleton<T>()
		{
			resolver.RegisterSingleton<T>();
		}
	}
}