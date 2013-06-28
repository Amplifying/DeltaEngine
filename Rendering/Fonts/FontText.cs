using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Rendering.Fonts
{
	public class FontText : Entity2D
	{
		public FontText(Font font, string text, Point position)
			: base(Rectangle.FromCenter(position, Size.One))
		{
			this.text = text;
			data = font.Data;
			data.Generate(text);
			Add(font.Image);
			Add(data.Glyphs);
			Add(data.DrawSize);
			Start<Render>();
		}

		private string text;
		private readonly FontData data;

		public string Text
		{
			get { return text; }
			set
			{
				text = value;
				UpdateText();
			}
		}

		private void UpdateText()
		{
			Remove<GlyphDrawData[]>();
			data.Generate(text);
			Add(data.Glyphs);
			Set(data.DrawSize);
		}

		public void SetPosition(Point position)
		{
			Set(Rectangle.FromCenter(position, Size.One));
		}

		public class Render : EventListener2D
		{
			public Render(Drawing drawing, ScreenSpace screen)
			{
				this.drawing = drawing;
				this.screen = screen;
				drawingFonts = new Dictionary<Image, SpriteInfo>();
			}

			private readonly Drawing drawing;
			private readonly ScreenSpace screen;
			private readonly Dictionary<Image, SpriteInfo> drawingFonts;

			public override void ReceiveMessage(Entity2D entity, object message)
			{
				if (message is SortAndRender.AddToBatch)
					AddToBatch(entity);
			}

			private void AddToBatch(Entity2D text)
			{
				var size = text.Get<Size>();
				var position = screen.ToPixelSpaceRounded(text.DrawArea.Center) -
					new Point((float)Math.Round(size.Width / 2), (float)Math.Round(size.Height / 2));
				AddVerticesAndIndices(text.Get<GlyphDrawData[]>(), position, text.Color, text.Get<Image>());
			}

			private void AddVerticesAndIndices(GlyphDrawData[] glyphs, Point position, Color color,
				Image image)
			{
				foreach (GlyphDrawData glyph in glyphs)
					AddVerticesAndIndicesForGlyph(glyph, position, color, image);
			}

			private struct SpriteInfo
			{
				public List<VertexPositionColorTextured> vertices;
				public List<short> indices;
			}

			private void AddVerticesAndIndicesForGlyph(GlyphDrawData glyph, Point position, Color color,
				Image image)
			{
				var newVertices = new List<VertexPositionColorTextured>
				{
					new VertexPositionColorTextured(position + glyph.DrawArea.TopLeft, color, glyph.UV.TopLeft),
					new VertexPositionColorTextured(position + glyph.DrawArea.TopRight, color,
						glyph.UV.TopRight),
					new VertexPositionColorTextured(position + glyph.DrawArea.BottomRight, color,
						glyph.UV.BottomRight),
					new VertexPositionColorTextured(position + glyph.DrawArea.BottomLeft, color,
						glyph.UV.BottomLeft)
				};
				if (!drawingFonts.ContainsKey(image))
					drawingFonts.Add(image,
						new SpriteInfo { vertices = newVertices, indices = new List<short> { 0, 1, 2, 0, 2, 3 } });
				else
					AddNewVerticesToList(image, newVertices);
			}

			private void AddNewVerticesToList(Image image, List<VertexPositionColorTextured> newVertices)
			{
				SpriteInfo spriteInfo;
				drawingFonts.TryGetValue(image, out spriteInfo);
				AddIndicesForGlyph(spriteInfo);
				foreach (var vertice in newVertices)
					spriteInfo.vertices.Add(vertice);
			}

			private static void AddIndicesForGlyph(SpriteInfo spriteInfo)
			{
				spriteInfo.indices.Add((short)spriteInfo.vertices.Count);
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 1));
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 2));
				spriteInfo.indices.Add((short)spriteInfo.vertices.Count);
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 2));
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 3));
			}

			public override void ReceiveMessage(object message)
			{
				if (!(message is SortAndRender.RenderBatch))
					return;

				foreach (var font in drawingFonts)
					RenderGraphics(font.Key, font.Value.vertices, font.Value.indices);

				drawingFonts.Clear();
			}

			private void RenderGraphics(Image image, List<VertexPositionColorTextured> vertices,
				List<short> indices)
			{
				var vertexArray = new VertexPositionColorTextured[vertices.Count + 1];
				for (int i = 0; i < vertices.Count; ++i)
					vertexArray[i] = vertices[i];

				var indicesArray = new short[indices.Count + 1];
				for (int i = 0; i < indices.Count; ++i)
					indicesArray[i] = indices[i];

				drawing.EnableTexturing(image);
				drawing.SetIndices(indicesArray, indicesArray.Length);
				drawing.DrawVerticesForSprite(VerticesMode.Triangles, vertexArray);
			}
		}
	}
}