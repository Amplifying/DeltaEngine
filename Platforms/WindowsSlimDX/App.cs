using System;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Initializes the SlimDX resolver for Windows DirectX 9. To execute the app call Run.
	/// </summary>
	public abstract class App
	{
		protected App()
		{
			resolver.CreateEntitySystemAndAddAsRunner();
		}

		private readonly SlimDXResolver resolver = new SlimDXResolver();

		public void Run(Action optionalRunCode = null)
		{
			resolver.Run(optionalRunCode);
			resolver.Dispose();
		}
	}
}