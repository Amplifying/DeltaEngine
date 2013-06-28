using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;

namespace Blobs.Creatures
{
	/// <summary>
	/// A ball which responds to gravity and deforms as it bounces
	/// </summary>
	public class RubberBall : Ellipse
	{
		public RubberBall()
			: base(Rectangle.Zero, Color.White)
		{
			Stop<RenderOutline>();
		}

		public void SetDamping(float setProportionalDamping, float setAbsoluteDamping)
		{
			proportionalDamping = setProportionalDamping;
			absoluteDamping = setAbsoluteDamping;
		}

		private float proportionalDamping = DefaultProportionalDamping;
		private float absoluteDamping = DefaultAbsoluteDamping;

		//TODO: Should be dependent on the collision material
		private const float DefaultProportionalDamping = 0.85f;
		private const float DefaultAbsoluteDamping = 0.06f;

		public virtual void Run()
		{
			float delta = Time.Current.Delta;
			if (delta > MaxDelta)
				delta = MaxDelta;

			AdjustVelocity(delta);
			AdjustPosition(delta);
			ProcessGrowth(delta);
			ProcessDeath(delta);
		}

		private const float MaxDelta = 1 / 30.0f;

		private void AdjustVelocity(float delta)
		{
			velocity += Gravity * delta;
		}

		private Point velocity;

		public Point Velocity
		{
			get { return velocity; }
			set
			{
				velocity = value;
				hasCollisionOccurred = false;
			}
		}

		private static readonly Point Gravity = new Point(0.0f, 4.0f);

		private void AdjustPosition(float delta)
		{
			PreviousFrameCenter = Center;
			elapsed += delta;
			if (hasCollisionOccurred)
				BounceOrRest();

			if (isBouncing)
				ContinueBounce();
			else
				Center += Velocity * delta;

			hasCollisionOccurred = false;
		}

		public Point PreviousFrameCenter { get; private set; }
		private float elapsed;

		public Collision Collision
		{
			get { return collision; }
			set
			{
				if (isBouncing)
					return;

				if (Velocity.DotProduct(value.Normal) > 0)
					return;

				collision = value;
				StoreCollision();
			}
		}

		private void StoreCollision()
		{
			hasCollisionOccurred = true;
			incidentDirection = -Velocity;
			incidentDirection.Normalize();
			collision.Normal.Normalize();
			reflectionDirection = 2 * collision.Normal * incidentDirection.DotProduct(collision.Normal) -
				incidentDirection;
		}

		private bool hasCollisionOccurred;
		private Collision collision;
		private Point incidentDirection;
		private Point reflectionDirection;
		private bool isBouncing;

		private void BounceOrRest()
		{
			speed = (float)Math.Sqrt(Velocity.X * Velocity.X + Velocity.Y * Velocity.Y);
			if (lastObject != collision.Object || speed > MinBounceSpeed || elapsed > MinBounceElapsed)
				BeginBounce();
			else
				Rotation = 0.0f;

			velocity = Point.Zero;
			elapsed = 0.0f;
		}

		private float speed;
		private const float MinBounceSpeed = 0.2f;
		private const float MinBounceElapsed = 0.2f;

		private void BeginBounce()
		{
			Rotation = 90.0f + MathExtensions.Atan2(collision.Normal.Y, collision.Normal.X);
			bounceRadiusX = RadiusX;
			bounceRadiusY = RadiusY;
			maxBounceRadiusX = RadiusX * (1 + speed / BounceStiffness);
			maxBounceRadiusY = RadiusY / (1 + speed / BounceStiffness);
			bounceDuration = MinBounceDuration * (1 + speed / BounceStiffness);
			lastObject = collision.Object;
			isBouncing = true;
		}

		private float bounceRadiusX;
		private float bounceRadiusY;
		private float maxBounceRadiusX;
		private float maxBounceRadiusY;
		private const float BounceStiffness = 4.0f;
		private float bounceDuration;
		private const float MinBounceDuration = 0.05f;
		private object lastObject;

		private void ContinueBounce()
		{
			if (elapsed < bounceDuration * 0.5f)
				IncreaseSquash();
			else
				DecreaseSquash();

			Center = collision.Point + RadiusY * collision.Normal;
			if (elapsed >= bounceDuration)
				EndBounce();
		}

		private void IncreaseSquash()
		{
			RadiusX = MathExtensions.Lerp(bounceRadiusX, maxBounceRadiusX, elapsed * 2 / bounceDuration);
			RadiusY = MathExtensions.Lerp(bounceRadiusY, maxBounceRadiusY, elapsed * 2 / bounceDuration);
		}

		private void DecreaseSquash()
		{
			RadiusX = MathExtensions.Lerp(maxBounceRadiusX, bounceRadiusX,
				(2 * elapsed / bounceDuration) - 1);
			RadiusY = MathExtensions.Lerp(maxBounceRadiusY, bounceRadiusY,
				(2 * elapsed / bounceDuration) - 1);
		}

		private void EndBounce()
		{
			velocity = reflectionDirection * DampenSpeed(speed);
			RadiusX = bounceRadiusX;
			RadiusY = bounceRadiusY;
			elapsed = 0.0f;
			isBouncing = false;
		}

		private float DampenSpeed(float speedComponent)
		{
			speedComponent *= proportionalDamping;
			if (Math.Abs(speedComponent) > absoluteDamping)
				return speedComponent - Math.Sign(speedComponent) * absoluteDamping;

			return 0.0f;
		}

		private void ProcessGrowth(float delta)
		{
			if (amountToGrow > 0)
				GrowBy(GetGrowth(delta));
		}

		private float amountToGrow;

		private float GetGrowth(float delta)
		{
			var growth = GrowthRate * delta;
			amountToGrow -= growth;
			if (amountToGrow > 0)
				return growth;

			growth += amountToGrow;
			amountToGrow = 0;
			return growth;
		}

		private const float GrowthRate = 0.05f;

		private void GrowBy(float growth)
		{
			RadiusX += growth;
			RadiusY += growth;
			bounceRadiusX += growth;
			bounceRadiusY += growth;
			maxBounceRadiusX += growth;
			maxBounceRadiusY += growth;
			Center = new Point(Center.X, Center.Y - growth);
		}

		public void Grow(float growth)
		{
			amountToGrow += growth;
		}

		private void ProcessDeath(float delta)
		{
			if (!IsDying)
				return;

			deathElapsed += delta;
			Color = new Color(Color.R, Color.G, Color.B,
				(byte)MathExtensions.Lerp(255, 128, deathElapsed / DeathDuration));
			Rotation += delta * rotationSpeed;
			Shrink();
			if (deathElapsed < DeathDuration)
				return;

			IsDying = false;
			HasDied = true;
		}

		public bool IsDying { get; private set; }
		public bool HasDied { get; private set; }
		private float deathElapsed;
		private float rotationSpeed;
		private const float DeathDuration = 1.0f;

		private void Shrink()
		{
			float size = MathExtensions.Lerp(deathRadius, 0, deathElapsed / DeathDuration);
			Radius = size;
			bounceRadiusX = size;
			bounceRadiusY = size;
			maxBounceRadiusX = size;
			maxBounceRadiusY = size;
		}

		private float deathRadius;

		public void Die()
		{
			deathRadius = Radius;
			rotationSpeed = Randomizer.Current.Get(-720.0f, 720.0f);
			Velocity = new Point(Randomizer.Current.Get(-0.4f, 0.4f),
				Randomizer.Current.Get(-0.4f, -1.2f));
			IsDying = true;
		}
	}
}