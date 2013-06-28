using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Shapes;

namespace Blobs.Creatures
{
	/// <summary>
	/// A blob that can be catapulted around by mouse drags
	/// </summary>
	public class Player : Blob
	{
		public Player(ScreenSpace screen, InputCommands input)
			: base(screen, input)
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
				AlterAim(Center, mouse.Position);
		}

		private Line2D aim;

		private void AlterAim(Point center, Point mousePosition)
		{
			aim.EndPoint = mousePosition;
			UpdateArrow1(center, mousePosition);
			UpdateArrow2(center, mousePosition);
		}

		private void UpdateArrow1(Point center, Point mousePosition)
		{
			arrow1.StartPoint = mousePosition;
			arrow1.EndPoint = center + center.DirectionTo(mousePosition) * ArrowLength;
			arrow1.EndPoint = arrow1.EndPoint.RotateAround(center, -5);
		}

		private void UpdateArrow2(Point center, Point mousePosition)
		{
			arrow2.StartPoint = mousePosition;
			arrow2.EndPoint = center + center.DirectionTo(mousePosition) * ArrowLength;
			arrow2.EndPoint = arrow2.EndPoint.RotateAround(center, 5);
		}

		private Line2D arrow1;
		private Line2D arrow2;
		private const float ArrowLength = 0.9f;

		private void BeginDrag(Mouse mouse)
		{
			if (Velocity != Point.Zero)
				return;

			aim = new Line2D(Center, Center, Color.Red) { RenderLayer = ArrowRenderLayer };
			arrow1 = new Line2D(Center, Center, Color.Red) { RenderLayer = ArrowRenderLayer };
			arrow2 = new Line2D(Center, Center, Color.Red) { RenderLayer = ArrowRenderLayer };
		}

		private const int ArrowRenderLayer = 9;

		private void EndDrag(Mouse obj)
		{
			if (aim == null)
				return;

			Velocity = LaunchVelocityFactor * (aim.EndPoint - aim.StartPoint);
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