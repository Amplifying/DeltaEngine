using System;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Initializes the XNA resolver and the window to get started. To execute the app call Run.
	/// </summary>
	public abstract class App
	{
		protected App()
		{
			resolver.CreateEntitySystemAndAddAsRunner();
		}

		private readonly XnaResolver resolver = new XnaResolver();

		public void Run(Action optionalRunCode = null)
		{
			resolver.Run(optionalRunCode);
			resolver.Dispose();
		}
	}
}