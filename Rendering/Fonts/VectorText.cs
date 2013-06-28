using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Rendering.Fonts
{
	/// <summary>
	/// Draws vector style text in 2D space
	/// </summary>
	public class VectorText : Entity2D
	{
		public VectorText(string text, Point position)
			: base(Rectangle.FromCenter(position, new Size(0.05f)))
		{
			Add(CalculateLinePoints(text));
			Start<Render>();
		}

		private List<Point> CalculateLinePoints(string text)
		{
			var points = new List<Point>();
			caretPosition = new Point(-CharacterWidth * text.Length / 2, -CharacterWidth / 2);
			foreach (char c in text)
				CalculateCharLines(points, c);

			return points;
		}

		private Point caretPosition = Point.Zero;
		private const float CharacterWidth = 0.64f;

		private void CalculateCharLines(List<Point> points, char c)
		{
			if (!char.IsWhiteSpace(c))
				points.AddRange(characterLines.GetPoints(c).Select(point => caretPosition + point));

			caretPosition.X += CharacterWidth;
		}

		private readonly VectorCharacterLines characterLines = new VectorCharacterLines();

		public class Render : EventListener2D
		{
			public Render(Drawing drawing, ScreenSpace screen)
			{
				this.drawing = drawing;
				this.screen = screen;
				vertices = new List<VertexPositionColor>();
			}

			private readonly Drawing drawing;
			private readonly ScreenSpace screen;
			private readonly List<VertexPositionColor> vertices;

			public override void ReceiveMessage(Entity2D entity, object message)
			{
				if (message is SortAndRender.AddToBatch)
					AddToBatch(entity);
			}

			private void AddToBatch(Entity2D text)
			{
				var area = text.DrawArea;
				AddLines(text.Get<List<Point>>(), area.Center, area.Height, text.Color);
			}

			private void AddLines(List<Point> points, Point position, float scale, Color color)
			{
				for (int num = 0; num < points.Count; num++)
					vertices.Add(
						new VertexPositionColor(screen.ToPixelSpaceRounded(points[num] * scale + position), color));
			}

			public override void ReceiveMessage(object message)
			{
				if (!(message is SortAndRender.RenderBatch) || vertices.Count == 0)
					return;

				DrawLines();
				vertices.Clear();
			}

			private void DrawLines()
			{
				var vertexArray = new VertexPositionColor[vertices.Count + 1];
				for (int i = 0; i < vertices.Count; ++i)
					vertexArray[i] = vertices[i];
				drawing.DisableTexturing();
				drawing.DrawVertices(VerticesMode.Lines, vertexArray);
			}

			public override Priority Priority
			{
				get { return Priority.Last; }
			}
		}
	}
}