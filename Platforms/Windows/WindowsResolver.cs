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
			RegisterSingleton<Renderer>();
			RegisterSingleton<StopwatchTime>();
			RegisterSingleton<Time>();
		}
	}
}