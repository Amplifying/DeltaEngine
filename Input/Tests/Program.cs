
using DeltaEngine.Platforms;

namespace DeltaEngine.Input.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			//new MouseTests().CountPressingAndReleasing(TestStarter.OpenGL);
			//new GamePadTests().CheckAvailable(TestWithAllFrameworks.GLFW);
			new GamePadTests().CheckLeftThumb();
			//new GamePadTests().CheckLeftTrigger(TestWithAllFrameworks.GLFW);
			//new GamePadTests().CheckRightThumb(TestWithAllFrameworks.GLFW);
			//new GamePadTests().CheckRightTrigger(TestWithAllFrameworks.GLFW);
		}
	}
}