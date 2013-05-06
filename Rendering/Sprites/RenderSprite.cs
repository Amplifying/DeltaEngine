using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// Responsible for rendering sprites
	/// </summary>
	public class RenderSprite : EntityListener
	{
		public RenderSprite(ScreenSpace screen)
		{
			this.screen = screen;
		}

		private readonly ScreenSpace screen;

		public void Handle(List<Entity> entities) {}

		public void ReceiveMessage(Entity entity, object message)
		{
			if (message is SortAndRenderEntity2D.TimeToRender)
				Render(entity);
		}

		private void Render(Entity entity)
		{
			var drawArea = entity.Get<Rectangle>();
			var rotation = entity.Get<Rotation>().Value;
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
			image.Draw(vertices);
		}

		private VertexPositionColorTextured GetVertex(Point position, Point uv, Entity entity)
		{
			return new VertexPositionColorTextured(screen.ToPixelSpaceRounded(position),
				entity.Get<Color>(), uv);
		}

		public EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.Normal; }
		}
	}
}