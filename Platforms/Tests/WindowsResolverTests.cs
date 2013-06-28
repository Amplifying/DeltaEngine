using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class WindowsResolverTests
	{
		//ncrunch: no coverage start
		[TestFixtureSetUp]
		public void CreateWindowsResolver()
		{
			resolver = new EmptyWindowsResolver();
			Assert.NotNull(resolver);
		}

		private EmptyWindowsResolver resolver;

		[TestFixtureTearDown]
		public void DisposeWindowsResolver()
		{
			resolver.Dispose();
		}

		private class EmptyWindowsResolver : AutofacStarter
		{
			public void Register(object instance)
			{
				RegisterInstance(instance);
			}
		}

		[Test, Category("Slow")]
		public void RegisterNonRenderableObject()
		{
			var rect = new Rectangle(Point.Half, Size.Half);
			resolver.Register(rect);
		}

		[Test, Category("Slow")]
		public void RegisterRenderableObject()
		{
			using (var device = new EmptyDevice())
			{
				device.Run();
				device.Present();
				resolver.Register(device);
				resolver.Register(new EmptyDrawing(device));
				resolver.Register(new Line2D(Point.One, Point.Zero, Color.Red));
			}
		}

		private class EmptyDevice : Device
		{
			public void Run() {}
			public void Present() {}
			public void Dispose() {}
			public void SetProjectionMatrix(Matrix matrix) {}
			public void SetModelViewMatrix(Matrix matrix) {}
		}

		private class EmptyDrawing : Drawing
		{
			public EmptyDrawing(Device device)
				: base(device) {}
			
			//ncrunch: no coverage start
			public override void Dispose() {}
			public override void EnableTexturing(Image image) {}
			public override void DisableTexturing() {}
			public override void SetBlending(BlendMode blendMode) {}
			public override void SetIndices(short[] indices, int usedIndicesCount) {}
			public override void DisableIndices() {}
			public override void DrawVerticesForSprite(VerticesMode mode,
				VertexPositionColorTextured[] vertices) {}
			public override void DrawVertices(VerticesMode mode, VertexPositionColor[] vertices) {}
		}
	}
}