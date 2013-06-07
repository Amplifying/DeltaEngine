using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Physics2D
{
	/// <summary>
	/// Component for any Entity moving around in 2D space with velocity limited to a maximum value
	/// It can be accelerated (or decelerated, which is the same) by a vector, by magnitude and direction,
	/// or by a scalar factor (if it 
	/// </summary>
	public class Velocity2D
	{
		public Velocity2D(Point velocity, float maximumVelocity)
		{
			this.velocity = velocity;
			this.maximumVelocity = maximumVelocity;
		}

		public Point velocity;
		public readonly float maximumVelocity;

		public void Accelerate(Point acceleration2D)
		{
			velocity += acceleration2D;

			float scalarVelocity = CalculateScalar();
			ScaleDownIfExceedingMaxVelocity(scalarVelocity);
		}

		private void ScaleDownIfExceedingMaxVelocity(float scalarVelocity)
		{
			if (!(maximumVelocity <= scalarVelocity))
				return;
			float reductionFactor = maximumVelocity / scalarVelocity;
			velocity.X *= reductionFactor;
			velocity.Y *= reductionFactor;
		}

		public void Accelerate(float magnitude, float degreeAngle)
		{
			velocity.X += MathExtensions.Sin(degreeAngle) * magnitude;
			velocity.Y -= MathExtensions.Cos(degreeAngle) * magnitude;

			float scalarVelocity = CalculateScalar();
			ScaleDownIfExceedingMaxVelocity(scalarVelocity);
		}

		public void Accelerate(float factor)
		{
			velocity.X *= factor;
			velocity.Y *= factor;
			var scalarVelocity = CalculateScalar();
			ScaleDownIfExceedingMaxVelocity(scalarVelocity);
		}

		private float CalculateScalar()
		{
			return (float)Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y);
		}
	}
}