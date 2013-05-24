using System;
using Blobs.Creatures;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Blobs.Tests.Creatures
{
	public class BlobTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void Bounce(Type resolver)
		{
			Blob blob = null;
			Start(resolver, (Blob b) =>
			{
				blob = b;
				blob.Radius = 0.05f;
				blob.Center = new Point(0.1f, 0.2f);
				blob.Color = Color.Blue;
				blob.Velocity = new Point(0.2f, 0.0f);
			}, () =>
			{
				if (blob.Center.Y + blob.Radius > 0.8f)
					blob.Collision = new Collision(new Point(blob.Center.X, 0.8f), -Point.UnitY, null);

				if (blob.Center.X + blob.Radius > 0.6f)
					blob.Collision = new Collision(new Point(0.6f, blob.Center.Y), -Point.UnitX, null);
			});
		}

		[VisualTest]
		public void CameraZoomsInOnBlob(Type resolver)
		{
			Blob blob = null;
			Camera2DControlledQuadraticScreenSpace camera = null;
			float elapsed = 0.0f;
			Start(resolver, (Blob b, Camera2DControlledQuadraticScreenSpace c) =>
			{
				blob = SetupBlob(b);
				camera = c;
			}, (Window window) =>
			{
				if (blob.Center.Y + blob.Radius > 0.8f)
					blob.Collision = new Collision(new Point(blob.Center.X, 0.8f), -Point.UnitY, null);

				elapsed += Time.Current.Delta;
				if (elapsed > 5.0f)
					camera.LookAt = Point.Lerp(Point.Half, new Point(blob.Center.X, 0.75f),
						0.2f * (elapsed - 5.0f));
				if (elapsed > 8.0f)
					camera.Zoom += 0.02f;
				window.Title = "FPS: " + Time.Current.Fps;
			});
		}

		private static Blob SetupBlob(Blob blob)
		{
			blob.Radius = 0.05f;
			blob.Center = new Point(0.1f, 0.2f);
			blob.Color = Color.Blue;
			blob.Velocity = new Point(0.2f, 0.0f);
			return blob;
		}

		[VisualTest]
		public void DrawSinkingRotatingBlob(Type resolver)
		{
			Blob blob = null;
			Start(resolver, (Blob b) => { blob = SetupBlob(b); }, () =>
			{
				blob.Velocity = Point.Zero;
				blob.Rotation += 45 * Time.Current.Delta;
			});
		}

		[VisualTest]
		public void BlobBouncingOffPlatformZoomOut(Type resolver)
		{
			Blob blob = null;
			Platform platform = null;
			Camera2DControlledQuadraticScreenSpace camera = null;
			Start(resolver, (Camera2DControlledQuadraticScreenSpace c, Blob b) =>
			{
				blob = SetupBlob(b);
				camera = c;
				platform = AddPlatform();
			}, () =>
			{
				camera.Zoom = MathExtensions.Max(camera.Zoom - Time.Current.Delta / 10f, 0.0f);
				platform.CheckForCollision(blob);
			});
		}

		private static Platform AddPlatform()
		{
			var platform = new Platform(new Rectangle(0.2f, 0.8f, 0.6f, 0.19f)) { Color = Color.Green };
			return platform;
		}
	}
}