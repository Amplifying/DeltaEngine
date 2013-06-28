using Blobs.Creatures;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.ScreenSpaces;
using NUnit.Framework;

namespace Blobs.Tests.Creatures
{
	public class BlobTests : TestWithMocksOrVisually
	{
		[Test]
		public void Bounce()
		{
			var blob = new Blob(Resolve<ScreenSpace>(), Resolve<InputCommands>());
			blob.Radius = 0.05f;
			blob.Center = new Point(0.1f, 0.2f);
			blob.Color = Color.Blue;
			blob.Velocity = new Point(0.2f, 0.0f);
			if (blob.Center.Y + blob.Radius > 0.8f)
				blob.Collision = new Collision(new Point(blob.Center.X, 0.8f), -Point.UnitY, null);
			if (blob.Center.X + blob.Radius > 0.6f)
				blob.Collision = new Collision(new Point(0.6f, blob.Center.Y), -Point.UnitX, null);
		}

		[Test]
		public void CameraZoomsInOnBlob()
		{
			float elapsed = 0.0f;
			var camera = new Camera2DControlledQuadraticScreenSpace(Resolve<Window>());
			var blob = new Blob(camera, Resolve<InputCommands>());
			blob = SetupBlob(blob);
			var window = Resolve<Window>();
			if (blob.Center.Y + blob.Radius > 0.8f)
				blob.Collision = new Collision(new Point(blob.Center.X, 0.8f), -Point.UnitY, null);

			elapsed += Time.Current.Delta;
			if (elapsed > 5.0f)
				camera.LookAt = Point.Lerp(Point.Half, new Point(blob.Center.X, 0.75f),
					0.2f * (elapsed - 5.0f));
			if (elapsed > 8.0f)
				camera.Zoom += 0.02f;
			window.Title = "FPS: " + Time.Current.Fps;
		}

		private static Blob SetupBlob(Blob blob)
		{
			blob.Radius = 0.05f;
			blob.Center = new Point(0.1f, 0.2f);
			blob.Color = Color.Blue;
			blob.Velocity = new Point(0.2f, 0.0f);
			return blob;
		}

		[Test]
		public void DrawSinkingRotatingBlob()
		{
			var blob = new Blob(new Camera2DControlledQuadraticScreenSpace(Resolve<Window>()), 
				Resolve<InputCommands>());
			blob.Velocity = Point.Zero;
			blob.Rotation += 45 * Time.Current.Delta;
		}

		[Test]
		public void BlobBouncingOffPlatformZoomOut()
		{
			var platform = AddPlatform();
			var camera = new Camera2DControlledQuadraticScreenSpace(Resolve<Window>());
			var blob = SetupBlob(new Blob(camera, Resolve<InputCommands>()));
			camera.Zoom = MathExtensions.Max(camera.Zoom - Time.Current.Delta / 10f, 0.0f);
			platform.CheckForCollision(blob);
		}

		private static Platform AddPlatform()
		{
			var platform = new Platform(new Rectangle(0.2f, 0.8f, 0.6f, 0.19f)) { Color = Color.Green };
			return platform;
		}
	}
}