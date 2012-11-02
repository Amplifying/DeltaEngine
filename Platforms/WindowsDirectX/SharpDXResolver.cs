using DeltaEngine.Graphics.SharpDX;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Windows SharpDX config (graphics, sound, input) for any Delta Engine application or test.
	/// </summary>
	public class SharpDXResolver : WindowsResolver
	{
		public SharpDXResolver()
		{
			RegisterSingleton<SharpDXDevice>();
			RegisterSingleton<SharpDXDrawing>();
		}
	}
}