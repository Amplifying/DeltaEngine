using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Rendering.Fonts
{
	public class FontText : Entity2D
	{
		public FontText(Font font, string text, Point position, float lineSpacingMultiplier = 1.0f,
			HorizontalAlignment horizontal = HorizontalAlignment.Centered,
			VerticalAlignment vertical = VerticalAlignment.Centered, bool wordWrap = true)
			: base(Rectangle.FromCenter(position, Size.One))
		{
			this.text = text;
			Add(font.Image);
			var drawSize = Size.Zero;
			data = font.Data;
			Add(data.GetGlyphDrawAreaAndUVs(text, lineSpacingMultiplier, horizontal, wordWrap,
				ref drawSize));
			Add(drawSize);
			Add<Render>();
		}

		private string text;
		private readonly FontData data;

		public string Text
		{
			get { return text; }
			set
			{
				Remove<GlyphDrawAreaAndUV[]>();
				text = value;
				Size drawSize = Size.Zero;
				Add(data.GetGlyphDrawAreaAndUVs(text, 1, HorizontalAlignment.Centered, true, ref drawSize));
				Set(drawSize);
			}
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
				var image = text.Get<Image>();
				var glyphs = text.Get<GlyphDrawAreaAndUV[]>();
				var size = text.Get<Size>();
				var position = screen.ToPixelSpaceRounded(text.DrawArea.Center) -
					new Point((float)Math.Round(size.Width / 2), (float)Math.Round(size.Height / 2));
				var color = text.Color;
				var vertices = new VertexPositionColorTextured[glyphs.Length * 4];
				var indices = new short[glyphs.Length * 6];
				int vertexIndex = 0;
				int indicesIndex = 0;
				foreach (var glyph in glyphs)
				{
					vertices[vertexIndex + 0] =
						new VertexPositionColorTextured(position + glyph.DrawArea.TopLeft, color,
							glyph.UV.TopLeft);
					vertices[vertexIndex + 1] =
						new VertexPositionColorTextured(position + glyph.DrawArea.TopRight, color,
							glyph.UV.TopRight);
					vertices[vertexIndex + 2] =
						new VertexPositionColorTextured(position + glyph.DrawArea.BottomRight, color,
							glyph.UV.BottomRight);
					vertices[vertexIndex + 3] =
						new VertexPositionColorTextured(position + glyph.DrawArea.BottomLeft, color,
							glyph.UV.BottomLeft);
					indices[indicesIndex++] = (short)vertexIndex;
					indices[indicesIndex++] = (short)(vertexIndex + 1);
					indices[indicesIndex++] = (short)(vertexIndex + 2);
					indices[indicesIndex++] = (short)vertexIndex;
					indices[indicesIndex++] = (short)(vertexIndex + 2);
					indices[indicesIndex++] = (short)(vertexIndex + 3);
					vertexIndex += 4;
				}
				drawing.EnableTexturing(image);
				drawing.SetIndices(indices, indices.Length);
				drawing.DrawVertices(VerticesMode.Triangles, vertices);
			}
		}
	}
}