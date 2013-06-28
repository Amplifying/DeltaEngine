using System;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Initializes the OpenTK resolver and the window to get started. To execute the app call Run.
	/// </summary>
	public class App : IDisposable
	{
		protected App()
		{
			resolver.CreateEntitySystemAndAddAsRunner();
		}

		internal App(Window windowToRegister)
		{
			resolver.RegisterInstance(windowToRegister);
			resolver.CreateEntitySystemAndAddAsRunner();
		}

		private readonly OpenTKResolver resolver = new OpenTKResolver();

		public void Run(Action optionalRunCode = null)
		{
			resolver.Run(optionalRunCode);
			Dispose();
		}

		public void Dispose()
		{
			resolver.Dispose();
		}

		public T Resolve<T>()
		{
			return resolver.Resolve<T>();
		}

		internal void RunFrame()
		{
			resolver.TryRunAllRunnersAndPresenters(null);
		}
	}
}