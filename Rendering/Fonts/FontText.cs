using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Rendering.Fonts
{
	public class FontText : Entity2D
	{
		public FontText(Font font, string text, Point position,
			HorizontalAlignment horizontal = HorizontalAlignment.Centered,
			VerticalAlignment vertical = VerticalAlignment.Centered)
			: base(Rectangle.FromCenter(position, Size.One))
		{
			this.text = text;
			Add(font.Image);
			var drawSize = Size.Zero;
			data = font.Data;
			Add(data.GetGlyphDrawAreaAndUVs(text, lineSpacingMultiplier, horizontal, isWordWrap,
				ref drawSize));
			Add(drawSize);
			Add<Render>();
		}

		private string text;
		private readonly FontData data;
		private float lineSpacingMultiplier = 1.0f;
		private bool isWordWrap = true;

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
			Size drawSize = Size.Zero;
			Add(data.GetGlyphDrawAreaAndUVs(text, lineSpacingMultiplier, HorizontalAlignment.Centered,
				true, ref drawSize));
			Set(drawSize);
		}

		public void SetPosition(Point position)
		{
			Set(Rectangle.FromCenter(position, Size.One));
		}

		public void SetLineSpacing(float multiplier)
		{
			lineSpacingMultiplier = multiplier;
			UpdateText();
		}

		public void SetWordWrap(bool wordWrap)
		{
			isWordWrap = wordWrap;
			UpdateText();
		}

		public class Render : EntityListener
		{
			public Render(Drawing drawing, ScreenSpace screen)
			{
				this.drawing = drawing;
				this.screen = screen;
			}

			private readonly Drawing drawing;
			private readonly ScreenSpace screen;

			public override void ReceiveMessage(Entity entity, object message)
			{
				if (message is SortAndRender.TimeToRender)
					RenderText(entity);
			}

			private void RenderText(Entity entity)
			{
				var text = entity as Entity2D;
				var size = text.Get<Size>();
				var position = screen.ToPixelSpaceRounded(text.DrawArea.Center) -
					new Point((float)Math.Round(size.Width / 2), (float)Math.Round(size.Height / 2));
				AddVerticesAndIndices(text.Get<GlyphDrawData[]>(), position, text.Color);
				RenderGraphics(text.Get<Image>());
			}

			private void AddVerticesAndIndices(GlyphDrawData[] glyphs, Point position, Color color)
			{
				verticesIndex = 0;
				vertices = new VertexPositionColorTextured[glyphs.Length * 4];
				indicesIndex = 0;
				indices = new short[glyphs.Length * 6];
				foreach (var glyph in glyphs)
					AddVerticesAndIndicesForGlyph(glyph, position, color);
			}

			private int verticesIndex;
			private VertexPositionColorTextured[] vertices;
			private int indicesIndex;
			private short[] indices;

			private void AddVerticesAndIndicesForGlyph(GlyphDrawData glyph, Point position, Color color)
			{
				AddVerticesForGlyph(glyph, position, color);
				AddIndicesForGlyph();
			}

			private void AddVerticesForGlyph(GlyphDrawData glyph, Point position, Color color)
			{
				vertices[verticesIndex + 0] =
					new VertexPositionColorTextured(position + glyph.DrawArea.TopLeft, color, glyph.UV.TopLeft);
				vertices[verticesIndex + 1] =
					new VertexPositionColorTextured(position + glyph.DrawArea.TopRight, color,
						glyph.UV.TopRight);
				vertices[verticesIndex + 2] =
					new VertexPositionColorTextured(position + glyph.DrawArea.BottomRight, color,
						glyph.UV.BottomRight);
				vertices[verticesIndex + 3] =
					new VertexPositionColorTextured(position + glyph.DrawArea.BottomLeft, color,
						glyph.UV.BottomLeft);
			}

			private void AddIndicesForGlyph()
			{
				indices[indicesIndex++] = (short)verticesIndex;
				indices[indicesIndex++] = (short)(verticesIndex + 1);
				indices[indicesIndex++] = (short)(verticesIndex + 2);
				indices[indicesIndex++] = (short)verticesIndex;
				indices[indicesIndex++] = (short)(verticesIndex + 2);
				indices[indicesIndex++] = (short)(verticesIndex + 3);
				verticesIndex += 4;
			}

			private void RenderGraphics(Image image)
			{
				drawing.EnableTexturing(image);
				drawing.SetIndices(indices, indices.Length);
				drawing.DrawVertices(VerticesMode.Triangles, vertices);
			}
		}
	}
}