using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Sprites
{
	/// <summary>
	/// A Sprite to which physics can be applied - eg. falling under gravity
	/// </summary>
	public class PhysicsSprite : Sprite
	{
		public PhysicsSprite(Image image, Rectangle drawArea)
			: this(image, drawArea, Color.White) {}

		public PhysicsSprite(Image image, Rectangle drawArea, Color color)
			: base(image, drawArea, color)
		{
			Add(new PhysicsObject { Gravity = DefaultGravity });
		}

		private static readonly Point DefaultGravity = new Point(0.0f, 2.0f);

		public Point Velocity
		{
			get { return Get<PhysicsObject>().Velocity; }
			set { Get<PhysicsObject>().Velocity = value; }
		}

		public Point Gravity
		{
			get { return Get<PhysicsObject>().Gravity; }
			set { Get<PhysicsObject>().Gravity = value; }
		}

		public float RotationSpeed
		{
			get { return Get<PhysicsObject>().RotationSpeed; }
			set { Get<PhysicsObject>().RotationSpeed = value; }
		}

		public float PhysicsDuration
		{
			get { return Get<PhysicsObject>().Duration; }
			set { Get<PhysicsObject>().Duration = value; }
		}
	}
}