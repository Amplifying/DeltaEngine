using DeltaEngine.Core;
using DeltaEngine.Rendering;

namespace DeltaEngine.Platforms.Windows
{
	/// <summary>
	/// Base resolver class for all windows framework implementations (OpenGL, DirectX or XNA).
	/// </summary>
	public abstract class WindowsResolver : AutofacResolver
	{
		protected WindowsResolver()
		{
			RegisterSingleton<FormsWindow>();
			RegisterSingleton<ScreenSpace>();
			RegisterSingleton<Renderer>();
			RegisterSingleton<StopwatchTime>();
			RegisterSingleton<Time>();
			RegisterSingleton<PseudoRandom>();
			RegisterSingleton<Content>();
		}

		protected override void RegisterInstanceAsRunnerOrPresenterIfPossible(object instance)
		{
			var renderable = instance as Renderable;
			if (renderable != null)
				Resolve<Renderer>().Add(renderable);

			base.RegisterInstanceAsRunnerOrPresenterIfPossible(instance);
		}
	}
}