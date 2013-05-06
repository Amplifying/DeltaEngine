using DeltaEngine.Platforms.All;

namespace Blobs.Tests
{
	internal static class Program
	{
		public static void Main()
		{
			new PlatformTests().DrawRotatingPlatform(TestWithAllFrameworks.OpenGL);
			//new RubberBallTests().Bounce(TestStarter.OpenGL);
			//new BlobTests().Bounce(TestStarter.OpenGL);
			//new BlobTests().CameraZoomsInOnBlob(TestStarter.OpenGL);
			//new BlobTests().DrawSinkingRotatingBlob(TestStarter.OpenGL);
			//new BlobTests().BlobBouncingOffPlatformZoomOut(TestStarter.OpenGL);
		}
	}
}