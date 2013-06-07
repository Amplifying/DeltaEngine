using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Particles
{
	internal class ParticleUpdater : Transition
	{
		protected override void EndTransition(Entity entity)
		{
			entity.IsActive = false;
		}

		protected override void UpdateEntityPosition(Entity2D entity, float percentage)
		{
			if (!entity.Contains<Position>())
				return;

			var particleInfo = entity.Get<ParticleInfo>();
			particleInfo.TimePassed += Time.Current.Delta;
			float timeLeft = particleInfo.LifeTime - particleInfo.TimePassed;
			Point newVelocity = Point.Lerp(particleInfo.StartDirection, particleInfo.Direction, percentage);

			if (newVelocity == Point.Zero)
				return;

			Point distanceToEnd = (newVelocity * timeLeft);
			Point walkDistance = distanceToEnd.Normalize() / 10 * Time.Current.Delta;
			entity.Center = entity.Center + (particleInfo.Speed * walkDistance);
		}
	}
}