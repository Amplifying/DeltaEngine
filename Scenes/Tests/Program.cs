using DeltaEngine.Platforms.Tests;
using DeltaEngine.Scenes.Tests.UserInterfaces;

namespace DeltaEngine.Scenes.Tests
{
	public static class Program
	{
		public static void Main()
		{
			//new ButtonTests().Draw(TestStarter.OpenGL);
			//new SceneTests().Draw(TestStarter.OpenGL);
			//new SceneFileTests().CreateSaveLoadAndShowScene(TestStarter.OpenGL);
			//new LabelTests().SaveAndLoad(TestStarter.OpenGL);
			new VectorTextControlTests().CreateSaveLoadAndShowRedHello(TestStarter.OpenGL);
		}
	}
}