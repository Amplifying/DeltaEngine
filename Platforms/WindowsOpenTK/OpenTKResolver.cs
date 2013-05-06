using DeltaEngine.Graphics.OpenTK;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.OpenTK;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	internal class OpenTKResolver : WindowsResolver
	{
		public OpenTKResolver()
		{
			RegisterSingleton<OpenTKDevice>();
			RegisterSingleton<OpenTKDrawing>();
			RegisterSingleton<OpenTKScreenshotCapturer>();
			RegisterSingleton<OpenTKSoundDevice>();
			RegisterSingleton<WindowsMouse>();
			RegisterSingleton<WindowsKeyboard>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			RegisterSingleton<CursorPositionTranslater>();
		}
	}
}