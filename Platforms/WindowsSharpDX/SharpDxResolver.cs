using DeltaEngine.Graphics.SharpDX;
using DeltaEngine.Input.SharpDX;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.SharpDX;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	internal class SharpDXResolver : WindowsResolver
	{
		public SharpDXResolver()
		{
			RegisterSingleton<SharpDXDevice>();
			RegisterSingleton<SharpDXDrawing>();
			RegisterSingleton<SharpDXScreenshotCapturer>();
			RegisterSingleton<XAudioDevice>();
			RegisterSingleton<SharpDXMouse>();
			RegisterSingleton<SharpDXKeyboard>();
			RegisterSingleton<SharpDXGamePad>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			RegisterSingleton<CursorPositionTranslater>();
			RegisterSingleton<SharpDXSystemInformation>();
		}
	}
}