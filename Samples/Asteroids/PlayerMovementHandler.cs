using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.ScreenSpaces;

namespace Asteroids
{
	/// <summary>
	/// Handler for the playership's specific behaviour
	/// </summary>
	public class PlayerMovementHandler : EntityHandler
	{
		public PlayerMovementHandler(ScreenSpace screen)
		{
			this.screen = screen;
		}

		private readonly ScreenSpace screen;

		public override void Handle(List<Entity> entities)
		{
			foreach (var entity in entities)
			{
				var nextRect = CalculateRectAfterMove(entity);
				MoveEntity(entity, nextRect);
				var velocity2D = entity.Get<Velocity2D>();
				velocity2D.velocity -= velocity2D.velocity * Constants.playerDecelFactor *
					Time.Current.Delta;
				entity.Set(velocity2D);
			}
		}

		private static Rectangle CalculateRectAfterMove(Entity entity)
		{
			return
				new Rectangle(
					entity.Get<Rectangle>().TopLeft + entity.Get<Velocity2D>().velocity * Time.Current.Delta,
					entity.Get<Rectangle>().Size);
		}

		private void MoveEntity(Entity entity, Rectangle rect)
		{
			//rect = GoThroughBorderIfNeeded(rect);
			StopAtBorder(entity);
			entity.Set(rect);
		}

		private void StopAtBorder(Entity entity)
		{
			var rect = entity.Get<Rectangle>();
			var vel = entity.Get<Velocity2D>();
			if (rect.Left < screen.Viewport.Left)
			{
				vel.velocity.X = 0.02f;
				rect.Left = screen.Viewport.Left;
			}
			if (rect.Right > screen.Viewport.Right)
			{
				vel.velocity.X = -0.02f;
				rect.Right = screen.Viewport.Right;
			}
			if (rect.Top < screen.Viewport.Top)
			{
				vel.velocity.Y = 0.02f;
				rect.Top = screen.Viewport.Top;
			}
			if (rect.Bottom > screen.Viewport.Bottom)
			{
				vel.velocity.Y = -0.02f;
				rect.Bottom = screen.Viewport.Bottom;
			}

			entity.Set(vel);
			entity.Set(rect);
		}

		private Rectangle GoThroughBorderIfNeeded(Rectangle rect)
		{
			if (rect.Left < screen.Viewport.Left)
				rect.Right = screen.Right;
			if (rect.Right > screen.Right)
				rect.Left = screen.Left;
			if (rect.Top < screen.Top)
				rect.Bottom = screen.Bottom;
			if (rect.Bottom > screen.Bottom)
				rect.Top = screen.Top;

			return rect;
		}

		public override EntityHandlerPriority Priority
		{
			get { return EntityHandlerPriority.First; }
		}
	}
}