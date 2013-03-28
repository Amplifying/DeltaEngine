using DeltaEngine.Graphics.SharpDX;
using DeltaEngine.Input.SharpDX;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.SharpDX;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	internal class SharpDxResolver : WindowsResolver
	{
		public SharpDxResolver()
		{
			RegisterSingleton<SharpDXDevice>();
			RegisterSingleton<SharpDXDrawing>();
			Register<SharpDXImage>();
			RegisterSingleton<XAudioDevice>();
			Register<XAudioSound>();
			Register<XAudioMusic>();
			Register<SharpDXVideo>();
			RegisterSingleton<SharpDXMouse>();
			RegisterSingleton<SharpDXKeyboard>();
			RegisterSingleton<SharpDXGamePad>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			RegisterSingleton<CursorPositionTranslater>();
		}
	}
}