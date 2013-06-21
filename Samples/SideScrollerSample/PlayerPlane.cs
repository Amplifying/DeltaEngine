using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Physics2D;
using DeltaEngine.Rendering.ScreenSpaces;

namespace SideScrollerSample
{
	public class PlayerPlane : Plane
	{
		/// <summary>
		/// Special behavior of the player's plane for use in the side-scrolling shoot'em'up.
		/// Can be controlled in its vertical position, fire shots and make the environment move.
		/// (It is so mighty that when it flies faster, in truth the world turns faster!)
		/// </summary>
		public PlayerPlane(Image playerTexture, Point initialPosition)
			: base(playerTexture, initialPosition, Color.PaleGreen)

		{
			Hitpoints = 3;
			verticalDecelerationFactor = 3.0f;
			verticalAccelerationFactor = 1.5f;
			RenderLayer = (int)DefRenderLayer.Player;
			PlayerFiredShot += point => { };
			Add(new Velocity2D(Point.Zero, MaximumSpeed));
			Add<PlayerMovement>();
			Add<MachineGunFire>();
		}

		protected class PlayerMovement : EntityHandler
		{
			public PlayerMovement(ScreenSpace screenSpace)
			{
				this.screenSpace = screenSpace;
				Filter = entity => entity is PlayerPlane;
			}

			private readonly ScreenSpace screenSpace;

			public override void Handle(Entity entity)
			{
				var playerPlane = entity as PlayerPlane;
				var newRect = CalculateRectAfterMove(playerPlane);
				MoveEntity(playerPlane, newRect);
				var velocity2D = playerPlane.Get<Velocity2D>();
				velocity2D.velocity.Y -= velocity2D.velocity.Y * playerPlane.verticalDecelerationFactor *
					Time.Current.Delta;
				playerPlane.Set(velocity2D);
				playerPlane.Rotation = RotationAccordingToVerticalSpeed(velocity2D.velocity);
			}

			private static Rectangle CalculateRectAfterMove(Entity entity)
			{
				var pointAfterVerticalMovement = new Point(entity.Get<Rectangle>().TopLeft.X,
					entity.Get<Rectangle>().TopLeft.Y +
						entity.Get<Velocity2D>().velocity.Y * Time.Current.Delta);

				return new Rectangle(pointAfterVerticalMovement, entity.Get<Rectangle>().Size);
			}

			private void MoveEntity(Entity entity, Rectangle rect)
			{
				StopAtBorderVertically(entity);
				entity.Set(rect);
			}

			private void StopAtBorderVertically(Entity entity)
			{
				var rect = entity.Get<Rectangle>();
				var vel = entity.Get<Velocity2D>();
				CheckStopTopBorder(rect, vel);
				CheckStopBottomBorder(rect, vel);
				entity.Set(vel);
				entity.Set(rect);
			}

			private void CheckStopTopBorder(Rectangle rect, Velocity2D vel)
			{
				if (rect.Top <= screenSpace.Viewport.Top && vel.velocity.Y < 0)
				{
					vel.velocity.Y = 0.02f;
					rect.Top = screenSpace.Viewport.Top;
				}
			}

			private void CheckStopBottomBorder(Rectangle rect, Velocity2D vel)
			{
				if (rect.Bottom >= screenSpace.Viewport.Bottom && vel.velocity.Y > 0)
				{
					vel.velocity.Y = -0.02f;
					rect.Bottom = screenSpace.Viewport.Bottom;
				}
			}

			private static float RotationAccordingToVerticalSpeed(Point vel)
			{
				return 50 * vel.Y / MaximumSpeed;
			}
		}

		private class MachineGunFire : EntityHandler
		{
			public MachineGunFire()
			{
				timeLastShot = Time.Current.Milliseconds;
				Filter = entity => entity is PlayerPlane;
			}

			private float timeLastShot;

			public override void Handle(Entity entity)
			{
				var playerPlane = entity as PlayerPlane;
				if (playerPlane.IsFireing && Time.Current.Milliseconds - 1 / Cadence > timeLastShot)
				{
					playerPlane.PlayerFiredShot(playerPlane.Get<Rectangle>().Center);
					timeLastShot = Time.Current.Milliseconds;
				}
			}
		}

		public bool IsFireing;
		public const float Cadence = 0.01f;
		public event Action<Point> PlayerFiredShot;
	}
}