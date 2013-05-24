using DeltaEngine.Core;

namespace Asteroids
{
	/// <summary>
	/// Simple rotation in degree and radians, normally for sprites or similar objects in 2D space
	/// </summary>
	public class TurnAngle
	{
		public TurnAngle(float degrees)
		{
			Degrees = degrees;
		}

		public float Degrees { get; set; }

		public float Radians
		{
			get { return Degrees.DegreesToRadians(); }
			set { Degrees = value.RadiansToDegrees(); }
		}

		public void DegreeRotate(float degreesToRotate)
		{
			Degrees = Degrees + degreesToRotate % 360;
		}
	}
}