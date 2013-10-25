using DeltaEngine.Core;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Initializes the OpenTK20Resolver and the window to get started. To execute the app call Run.
	/// </summary>
	public abstract class App
	{
		protected App() {}

		protected App(Window windowToRegister)
		{
			resolver.RegisterInstance(windowToRegister);
		}

		private readonly OpenTK20Resolver resolver = new OpenTK20Resolver();

		protected void Run()
		{
			resolver.Run();
			resolver.Dispose();
		}

		protected internal T Resolve<T>() where T : class
		{
			return resolver.Resolve<T>();
		}
	}
}