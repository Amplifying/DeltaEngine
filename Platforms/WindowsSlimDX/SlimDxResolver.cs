using DeltaEngine.Graphics.SlimDX;
using DeltaEngine.Input.SharpDX;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.SharpDX;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	internal class SlimDxResolver : WindowsResolver
	{
		public SlimDxResolver()
		{
			RegisterSingleton<SlimDXDevice>();
			RegisterSingleton<SlimDXDrawing>();
			Register<SlimDXImage>();
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