using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.ScreenSpaces;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// 2D sprite to be rendered, which is an image used as an Entity2D.
	/// </summary>
	public class Sprite : Entity2D
	{
		//ncrunch: no coverage start
		private Sprite()
			: base(Rectangle.Zero, Color.White) {}

		//ncrunch: no coverage end

		public Sprite(Image image)
			: this(image, Rectangle.Zero) {}

		public Sprite(Image image, Rectangle drawArea)
			: this(image, drawArea, Color.White) {}

		public Sprite(Image image, Color color)
			: this(image, Rectangle.Zero, color) {}

		public Sprite(Image image, Rectangle drawArea, Color color)
			: base(drawArea, color)
		{
			if (image == null)
				throw new NullReferenceException("image");

			Add(image);
			Add(image.HasAlpha ? BlendMode.Normal : BlendMode.Opaque);
			Add<Render>();
		}

		public Image Image
		{
			get { return Get<Image>(); }
			set { Set(value); }
		}

		public BlendMode BlendMode
		{
			get { return Get<BlendMode>(); }
			set { Set(value); }
		}

		/// <summary>
		/// Responsible for rendering sprites
		/// </summary>
		private class Render : EntityListener
		{
			public Render(ScreenSpace screen, Drawing drawing)
			{
				this.screen = screen;
				this.drawing = drawing;
				drawingSprites = new Dictionary<Image, SpriteInfo>();
			}

			private readonly ScreenSpace screen;
			private readonly Drawing drawing;
			private readonly Dictionary<Image, SpriteInfo> drawingSprites;

			public override void Handle(Entity entity)
			{
				if (entity.Get<Visibility>() == Visibility.Hide)
					return;

				drawingSprites.Clear();
				RenderSprite(entity);
				foreach (var sprite in drawingSprites)
					drawing.DrawQuad(sprite.Key, sprite.Value.vertices, sprite.Value.indices);
			}

			public override void ReceiveMessage(Entity entity, object message) {}

			private void RenderSprite(Entity entity)
			{
				var drawArea = entity.Get<Rectangle>();
				var rotation = entity.Get<float>();
				var center = drawArea.Center;
				var rotationCenter = entity.Contains<RotationCenter>()
					? entity.Get<RotationCenter>().Value : center;

				var newVertices = new List<VertexPositionColorTextured>
				{
					GetVertex(drawArea.TopLeft.RotateAround(rotationCenter, rotation), Point.Zero, entity),
					GetVertex(drawArea.TopRight.RotateAround(rotationCenter, rotation), Point.UnitX, entity),
					GetVertex(drawArea.BottomRight.RotateAround(rotationCenter, rotation), Point.One, entity),
					GetVertex(drawArea.BottomLeft.RotateAround(rotationCenter, rotation), Point.UnitY, entity)
				};
				var image = entity.Get<Image>();
				CheckIfNewImage(image, newVertices);
			}

			private void CheckIfNewImage(Image image, List<VertexPositionColorTextured> newVertices)
			{
				if (!drawingSprites.ContainsKey(image))
					drawingSprites.Add(image,
						new SpriteInfo { vertices = newVertices, indices = new List<short> { 0, 1, 2, 0, 2, 3 } });
				else
					AddNewVerticesToList(image, newVertices);
			}

			private void AddNewVerticesToList(Image image, List<VertexPositionColorTextured> newVertices)
			{
				SpriteInfo spriteInfo;
				drawingSprites.TryGetValue(image, out spriteInfo);
				SetIndicesOfNewVertices(spriteInfo);
				foreach (var vertice in newVertices)
					spriteInfo.vertices.Add(vertice);
			}

			private static void SetIndicesOfNewVertices(SpriteInfo spriteInfo)
			{
				spriteInfo.indices.Add((short)spriteInfo.vertices.Count);
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 1));
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 2));
				spriteInfo.indices.Add((short)spriteInfo.vertices.Count);
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 2));
				spriteInfo.indices.Add((short)(spriteInfo.vertices.Count + 3));
			}

			private VertexPositionColorTextured GetVertex(Point position, Point uv, Entity entity)
			{
				return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position),
					entity.Get<Color>(), uv);
			}

			private struct SpriteInfo
			{
				public List<VertexPositionColorTextured> vertices;
				public List<short> indices;
			}
		}
	}
}