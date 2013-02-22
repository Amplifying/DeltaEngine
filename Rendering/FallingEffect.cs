using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Simple falling particle effect, which is based on a sprite and disposes it after timout.
	/// </summary>
	public class FallingEffect : Sprite
	{
		public FallingEffect(Image image, Point position, Size size, float timeout = 1.0f)
			: base(image, Rectangle.FromCenter(position, size))
		{
			this.timeout = timeout;
		}

		public FallingEffect(Image image, Rectangle drawArea, float timeout = 1.0f)
			: base(image, drawArea)
		{
			this.timeout = timeout;
		}

		private readonly float timeout;

		protected override void Render(Renderer renderer, Time time)
		{
			MoveSprite(time);
			base.Render(renderer, time);
			CheckIfTimeToDispose(time);
		}

		private void MoveSprite(Time time)
		{
			Velocity += Gravity * time.CurrentDelta;
			DrawArea.Center += Velocity * time.CurrentDelta;
			Rotation += RotationSpeed * time.CurrentDelta;
		}

		public Point Velocity;
		public Point Gravity = DefaultGravity;
		private static readonly Point DefaultGravity = new Point(0.0f, 2.0f);
		public float RotationSpeed;

		private void CheckIfTimeToDispose(Time time)
		{
			elapsed += time.CurrentDelta;
			if (elapsed >= timeout)
				Dispose();
		}

		private float elapsed;
	}
}