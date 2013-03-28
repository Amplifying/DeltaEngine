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
			Register<OpenTKImage>();
			RegisterSingleton<OpenTKSoundDevice>();
			Register<OpenTKSound>();
			Register<OpenTKMusic>();
			RegisterSingleton<WindowsMouse>();
			RegisterSingleton<WindowsKeyboard>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			RegisterSingleton<CursorPositionTranslater>();
		}
	}
}