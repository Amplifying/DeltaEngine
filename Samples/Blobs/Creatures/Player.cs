using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering;
using DeltaEngine.Rendering.Shapes;

namespace Blobs.Creatures
{
	/// <summary>
	/// A blob that can be catapulted around by mouse drags
	/// </summary>
	public class Player : Blob
	{
		public Player(EntitySystem entitySystem, ScreenSpace screen, InputCommands input)
			: base(entitySystem, screen, input)
		{
			RespondToMouseDragging();
		}

		private void RespondToMouseDragging()
		{
			mouseMovementCommand = input.AddMouseMovement(StoreMousePosition);
			beginDragCommand = input.Add(MouseButton.Left, State.Pressing, BeginDrag);
			endDragCommand = input.Add(MouseButton.Left, State.Releasing, EndDrag);
		}

		private Command mouseMovementCommand;
		private Command beginDragCommand;
		private Command endDragCommand;

		private void StoreMousePosition(Mouse mouse)
		{
			if (aim != null)
				AlterAim(Center, camera.InverseTransform(mouse.Position));
		}

		private Line2D aim;

		private void AlterAim(Point center, Point mousePosition)
		{
			aim.End = mousePosition;
			UpdateArrow1(center, mousePosition);
			UpdateArrow2(center, mousePosition);
		}

		private void UpdateArrow1(Point center, Point mousePosition)
		{
			arrow1.Start = mousePosition;
			arrow1.End = center + center.DirectionTo(mousePosition) * ArrowLength;
			arrow1.End = arrow1.End.RotateAround(center, -5);
		}

		private void UpdateArrow2(Point center, Point mousePosition)
		{
			arrow2.Start = mousePosition;
			arrow2.End = center + center.DirectionTo(mousePosition) * ArrowLength;
			arrow2.End = arrow2.End.RotateAround(center, 5);
		}

		private Line2D arrow1;
		private Line2D arrow2;
		private const float ArrowLength = 0.9f;

		private void BeginDrag(Mouse mouse)
		{
			if (Velocity != Point.Zero)
				return;

			entitySystem.Add(
				aim = new Line2D(Center, Center, Color.Red) { RenderLayer = ArrowRenderLayer });
			entitySystem.Add(
				arrow1 = new Line2D(Center, Center, Color.Red) { RenderLayer = ArrowRenderLayer });
			entitySystem.Add(
				arrow2 = new Line2D(Center, Center, Color.Red) { RenderLayer = ArrowRenderLayer });
		}

		private const int ArrowRenderLayer = 9;

		private void EndDrag(Mouse obj)
		{
			if (aim == null)
				return;

			Velocity = LaunchVelocityFactor * (aim.End - aim.Start);
			aim.IsActive = false;
			arrow1.IsActive = false;
			arrow2.IsActive = false;
			aim = null;
		}

		private const float LaunchVelocityFactor = 5.0f;

		public bool CheckForCollision(Ellipse blob)
		{
			var direction = blob.Center.DirectionTo(Center);
			direction.Normalize();
			if (blob.Center.DistanceTo(Center) > blob.Radius + Radius)
				return false;

			CollidesWithBlob(blob, direction);
			return true;
		}

		private void CollidesWithBlob(Ellipse blob, Point direction)
		{
			Point collisionPoint = blob.Center + direction * blob.Radius;
			Collision = new Collision(collisionPoint, direction, this);
		}

		public override void Dispose()
		{
			input.Remove(mouseMovementCommand);
			input.Remove(beginDragCommand);
			input.Remove(endDragCommand);
			base.Dispose();
		}
	}
}