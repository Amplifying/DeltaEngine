using DeltaEngine.Graphics.SharpDX;
using DeltaEngine.Input.SharpDX;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.SharpDX;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Exchangable App entry point for Windows DirectX applications and visual tests.
	/// </summary>
	public class App : WindowsResolver
	{
		public App()
		{
			RegisterSingleton<SharpDXDevice>();
			RegisterSingleton<SharpDXDrawing>();
			Register<SharpDXImage>();
			RegisterSingleton<XAudioDevice>();
			Register<XAudioSound>();
			Register<XAudioMusic>();
			RegisterSingleton<SharpDXMouse>();
			RegisterSingleton<SharpDXKeyboard>();
			RegisterSingleton<SharpDXGamePad>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			RegisterSingleton<CursorPositionTranslater>();
		}
	}
}