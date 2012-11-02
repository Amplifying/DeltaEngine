using DeltaEngine.Graphics.OpenTK;
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
		}
	}
}