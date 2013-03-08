using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Tests;

namespace DeltaEngine.Graphics.Tests
{
	public class DrawingTests : TestStarter
	{
		[IntegrationTest]
		public void DrawVertices(Type resolver)
		{
			Start(resolver,
				(Drawing d) => d.DrawVertices(VerticesMode.Lines, new VertexPositionColor[4]));
		}

		[IntegrationTest]
		public void DrawVerticesWithIndices(Type resolver)
		{
			Start(resolver, (Drawing drawing) =>
			{
				drawing.SetIndices(new short[16], 16);
				drawing.DrawVertices(VerticesMode.Lines, new VertexPositionColor[4]);
			});
		}

		[IntegrationTest]
		public void SetIndices(Type resolver)
		{
			Start(resolver, (Drawing drawing) => drawing.SetIndices(new short[16], 16));
		}

		[IntegrationTest]
		public void SetIndicesTwice(Type resolver)
		{
			Start(resolver, (Device device, Window window, Drawing drawing) =>
			{
				drawing.SetIndices(new short[16], 16);
				drawing.SetIndices(new short[16], 16);
			});
		}

		[IntegrationTest]
		public void CheckCreatePositionColorBuffer(Type resolver)
		{
			Start(resolver, (Drawing drawing, Window window) =>
			{
				drawing.DrawVertices(VerticesMode.Triangles, new VertexPositionColor[4]);
				drawing.DrawVertices(VerticesMode.Triangles, new VertexPositionColor[4]);
			});
		}

		[VisualTest]
		public void ShowRedLine(Type resolver)
		{
			var vertices = new VertexPositionColor[2];
			Drawing remDrawing = null;
			Start(resolver, (Drawing drawing, Window window) =>
			{
				remDrawing = drawing;
				vertices[0] = new VertexPositionColor(Point.Zero, Color.Red);
				vertices[1] = new VertexPositionColor(window.ViewportPixelSize, Color.Red);
				window.ViewportSizeChanged +=
					size => vertices[1] = new VertexPositionColor(size, Color.Red);
			}, () => remDrawing.DrawVertices(VerticesMode.Lines, vertices));
		}

		[VisualTest]
		public void ShowYellowLineFullscreen(Type resolver)
		{
			var vertices = new VertexPositionColor[2];
			Drawing remDrawing = null;
			Start(resolver, (Drawing drawing, Window window) =>
			{
				window.SetFullscreen(new Size(640, 480));
				remDrawing = drawing;
				vertices[0] = new VertexPositionColor(Point.Zero, Color.Yellow);
				vertices[1] = new VertexPositionColor(window.ViewportPixelSize, Color.Yellow);
				window.ViewportSizeChanged +=
					size => vertices[1] = new VertexPositionColor(size, Color.Yellow);
			}, () => remDrawing.DrawVertices(VerticesMode.Lines, vertices));
		}
	}
}