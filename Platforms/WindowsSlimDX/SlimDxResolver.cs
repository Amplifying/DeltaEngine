using DeltaEngine.Graphics.SlimDX;
using DeltaEngine.Input.SlimDX;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.SlimDX;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	internal class SlimDXResolver : WindowsResolver
	{
		public SlimDXResolver()
		{
			RegisterSingleton<SlimDXDevice>();
			RegisterSingleton<SlimDXDrawing>();
			RegisterSingleton<SlimDXScreenshotCapturer>();
			RegisterSingleton<XAudioDevice>();
			RegisterSingleton<SlimDXMouse>();
			RegisterSingleton<SlimDXKeyboard>();
			RegisterSingleton<SlimDXGamePad>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			RegisterSingleton<CursorPositionTranslater>();
			RegisterSingleton<SlimDXSystemInformation>();
		}
	}
}