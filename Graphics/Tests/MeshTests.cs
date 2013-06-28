using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class MeshTests : TestWithMocksOrVisually
	{
		[Test]
		public void DrawRotatingSpikeBall()
		{
			var renderer = new MeshRenderer();
			Window.BackgroundColor = Color.CornflowerBlue;
			renderer.LoadMeshAndTexture("SpikeBall", "DeltaEngineLogo");
			RunCode = () => renderer.Draw(Resolve<Device>(), Resolve<Drawing>(), 200.0f);
		}

		private class MeshRenderer
		{
			public void LoadMeshAndTexture(string meshName, string textureName)
			{
				mesh = ContentLoader.Load<Mesh>(meshName);
				image = ContentLoader.Load<Image>(textureName);
			}

			private Mesh mesh;
			private Image image;

			public void Draw(Device device, Drawing draw, float cameraDistance)
			{
				draw.EnableTexturing(image);
				device.SetProjectionMatrix(Matrix.CreatePerspective(45, 360.0f / 640.0f, 0.5f, 200.0f));
				rotationY += RotationSpeed * Time.Current.Delta;
				var rotationMatrix = Matrix.CreateRotationX(RotationX) * Matrix.CreateRotationY(rotationY);
				var position = Matrix.TransformNormal(new Vector(0, 0, -cameraDistance), rotationMatrix);
				device.SetModelViewMatrix(Matrix.CreateLookAt(position, Vector.Zero, Vector.UnitY));
				mesh.Draw();
			}

			private float rotationY;
			private const float RotationSpeed = 45;
			private const float RotationX = 30;
		}

		[Test]
		public void DrawRotatingIceTower()
		{
			var renderer = new MeshRenderer();
			Window.BackgroundColor = Color.DarkGray;
			Window.ViewportPixelSize = new Size(1280, 720);
			renderer.LoadMeshAndTexture("CtTowersIceConeIcelady", "CtTowersIceConeIceladyDiff");
			RunCode = () => renderer.Draw(Resolve<Device>(), Resolve<Drawing>(), 40.0f);
		}

		[Test]
		public void CheckMeshNumberOfVertices()
		{
			Window.BackgroundColor = Color.Black;
			Assert.AreEqual(2203, ContentLoader.Load<Mesh>("SpikeBall").NumberOfVertices);
			Window.CloseAfterFrame();
		}
	}
}