using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering.Particels
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
			Point newVelocity = Point.Lerp(particleInfo.StartVelocity, particleInfo.Velocity, percentage);

			Point distanceToEnd = (newVelocity * timeLeft);
			Point walkdistance = distanceToEnd.Normalize() / 10 * Time.Current.Delta;
			entity.Center = entity.Center + (particleInfo.Speed * walkdistance);
		}

		public class ParticleInfo
		{
			public ParticleInfo(Point startVelocity, Point velocity, float speed, float lifeTime)
			{
				StartVelocity = startVelocity;
				Velocity = velocity;
				Speed = speed;
				LifeTime = lifeTime;
				TimePassed = 0;
			} 

			public Point StartVelocity { get; set; }
			public Point Velocity { get; set; }
			public float Speed { get; set; }
			public float LifeTime { get; set; }
			public float TimePassed { get; set; }
		}
	}
}