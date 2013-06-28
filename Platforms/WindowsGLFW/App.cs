using System;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Initializes the GLFW resolver and the window to get started. To execute the app call Run.
	/// </summary>
	public abstract class App
	{
		protected App()
		{
			resolver.CreateEntitySystemAndAddAsRunner();
		}

		private readonly GLFWResolver resolver = new GLFWResolver();

		public void Run(Action optionalRunCode = null)
		{
			resolver.Run(optionalRunCode);
			resolver.Dispose();
		}

		public T Resolve<T>()
		{
			return resolver.Resolve<T>();
		}
	}
}