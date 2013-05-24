using System;
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

		public Sprite(Image image, Rectangle drawArea)
			: this(image, drawArea, Color.White) {}

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
		public class Render : EntityListener
		{
			public Render(ScreenSpace screen, Drawing drawing)
			{
				this.screen = screen;
				this.drawing = drawing;
			}

			private readonly ScreenSpace screen;
			private readonly Drawing drawing;

			public override void ReceiveMessage(Entity entity, object message)
			{
				if (message is SortAndRender.TimeToRender)
					RenderSprite(entity);
			}

			private void RenderSprite(Entity entity)
			{
				var drawArea = entity.Get<Rectangle>();
				var rotation = entity.Get<float>();
				var center = drawArea.Center;
				var rotationCenter = entity.Contains<RotationCenter>()
					? entity.Get<RotationCenter>().Value : center;

				var vertices = new[]
				{
					GetVertex(drawArea.TopLeft.RotateAround(rotationCenter, rotation), Point.Zero, entity),
					GetVertex(drawArea.TopRight.RotateAround(rotationCenter, rotation), Point.UnitX, entity),
					GetVertex(drawArea.BottomRight.RotateAround(rotationCenter, rotation), Point.One, entity),
					GetVertex(drawArea.BottomLeft.RotateAround(rotationCenter, rotation), Point.UnitY, entity)
				};
				var image = entity.Get<Image>();
				drawing.DrawQuad(image, vertices);
			}

			private VertexPositionColorTextured GetVertex(Point position, Point uv, Entity entity)
			{
				return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position),
					entity.Get<Color>(), uv);
			}
		}
	}
}