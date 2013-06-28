using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering.Cameras;
using DeltaEngine.Rendering.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Shapes
{
	public class Line3DTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderCoordinateSystemCross()
		{
			new Line3D(-Vector.UnitX, Vector.UnitX * 3, Color.Red);
			new Line3D(-Vector.UnitY, Vector.UnitY * 3, Color.Green);
			new Line3D(-Vector.UnitZ, Vector.UnitZ * 3, Color.Blue);
		}

		[Test]
		public void RenderGrid()
		{
			const int GridSize = 10;
			const float GridScale = 0.5f;
			const float HalfGridSize = GridSize * 0.5f;
			var axisXz = new Point(-HalfGridSize, -HalfGridSize);
			for (int i = 0; i <= GridSize; i++, axisXz.X += 1, axisXz.Y += 1)
			{
				new Line3D(new Vector(-HalfGridSize * GridScale, 0.0f, axisXz.Y * GridScale),
					new Vector(HalfGridSize * GridScale, 0.0f, axisXz.Y * GridScale), Color.White);
				new Line3D(new Vector(axisXz.X * GridScale, 0.0f, -HalfGridSize * GridScale),
					new Vector(axisXz.X * GridScale, 0.0f, HalfGridSize * GridScale), Color.White);
			}
		}

		[Test]
		public void RenderRedLine()
		{
			Resolve<LookAtCamera>().Position = Vector.UnitZ;
			new Line3D(-Vector.UnitX, Vector.UnitX, Color.Red);
		}

		[Test]
		public void CreateLine3D()
		{
			var entity = new Line3D(Vector.Zero, Vector.One, Color.Red);
			Assert.AreEqual(Vector.Zero, entity.StartPoint);
			Assert.AreEqual(Vector.One, entity.EndPoint);
			Window.CloseAfterFrame();
		}

		[Test]
		public void SetLine3DPoints()
		{
			var entity = new Line3D(Vector.Zero, Vector.Zero, Color.Red)
			{
				StartPoint = Vector.UnitX,
				EndPoint = Vector.UnitY
			};
			Assert.AreEqual(Vector.UnitX, entity.StartPoint);
			Assert.AreEqual(Vector.UnitY, entity.EndPoint);
			Window.CloseAfterFrame();
		}

		[Test]
		public void SetLine3DPointList()
		{
			var entity = new Line3D(Vector.Zero, Vector.Zero, Color.Red)
			{
				Points = new List<Vector> { Vector.UnitZ, Vector.UnitY }
			};
			Assert.AreEqual(Vector.UnitZ, entity.StartPoint);
			Assert.AreEqual(Vector.UnitY, entity.EndPoint);
			Window.CloseAfterFrame();
		}
	}
}