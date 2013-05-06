using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class DrawingTests : TestWithAllFrameworks
	{
		[VisualTest, ApproveFirstFrameScreenshot]
		public void ShowRedLine(Type resolver)
		{
			var vertices = new VertexPositionColor[2];
			Drawing remDrawing = null;
			Start(resolver, (Drawing drawing, Window window) =>
			{
				remDrawing = drawing;
				DrawLine(window, vertices, Color.Red);
			}, () => remDrawing.DrawVertices(VerticesMode.Lines, vertices));
		}

		private static void DrawLine(Window window, IList<VertexPositionColor> vertices, Color color)
		{
			vertices[0] = new VertexPositionColor(Point.Zero, color);
			vertices[1] = new VertexPositionColor(window.ViewportPixelSize, color);
			window.ViewportSizeChanged += s => vertices[1] = new VertexPositionColor(s, Color.White);
		}

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

		//ncrunch: no coverage start
		[VisualTest, ApproveFirstFrameScreenshot, Ignore]
		public void ShowYellowLineFullscreen(Type resolver)
		{
			var vertices = new VertexPositionColor[2];
			Drawing remDrawing = null;
			Start(resolver, (Drawing drawing, Window window) =>
			{
				remDrawing = drawing;
				window.SetFullscreen(new Size(640, 480));
				DrawLine(window, vertices, Color.Yellow);
			}, () => remDrawing.DrawVertices(VerticesMode.Lines, vertices));
		}
	}
}