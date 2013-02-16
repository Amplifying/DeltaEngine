using DeltaEngine.Graphics.OpenTK;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.OpenTK;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Windows OpenTK config (graphics, sound, input) for any Delta Engine application or test.
	/// </summary>
	public class OpenTKResolver : WindowsResolver
	{
		public OpenTKResolver()
		{
			RegisterSingleton<OpenTKDevice>();
			RegisterSingleton<OpenTKDrawing>();
			Register<OpenTKImage>();
			RegisterSingleton<OpenTKSoundDevice>();
			Register<OpenTKSound>();
			RegisterSingleton<WindowsMouse>();
			RegisterSingleton<WindowsKeyboard>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			RegisterSingleton<CursorPositionTranslater>();
		}
	}
}