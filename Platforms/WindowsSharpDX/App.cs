using System;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Initializes the SharpDX resolver for Windows DirectX 11. To execute the app call Run.
	/// </summary>
	public abstract class App
	{
		protected App()
		{
			resolver.CreateEntitySystemAndAddAsRunner();
		}

		private readonly SharpDXResolver resolver = new SharpDXResolver();

		public void Run(Action optionalRunCode = null)
		{
			resolver.Run(optionalRunCode);
			resolver.Dispose();
		}
	}
}