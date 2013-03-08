using DeltaEngine.Graphics.OpenTK;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.OpenTK;
using DeltaEngine.Platforms.Windows;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Provides the easily exchangeable App.Start function for OpenTK applications and visual tests.
	/// </summary>
	public class App : WindowsResolver
	{
		public App()
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