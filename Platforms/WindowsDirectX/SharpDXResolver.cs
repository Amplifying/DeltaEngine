using DeltaEngine.Graphics.SharpDX;
using DeltaEngine.Input.SharpDX;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.SharpDX;
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
			Register<SharpDXImage>();
			RegisterSingleton<XAudioDevice>();
			Register<XAudioSound>();
			RegisterSingleton<SharpDXMouse>();
			RegisterSingleton<SharpDXKeyboard>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			RegisterSingleton<CursorPositionTranslater>();
		}
	}
}