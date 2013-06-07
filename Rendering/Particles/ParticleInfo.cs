using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Particles
{
	internal class ParticleInfo
	{
		public ParticleInfo(Point startVelocity, Point velocity, float speed, float lifeTime)
		{
			StartDirection = startVelocity;
			Direction = velocity;
			Speed = speed;
			LifeTime = lifeTime;
			TimePassed = 0;
		}

		public Point StartDirection { get; set; }
		public Point Direction { get; set; }
		public float Speed { get; set; }
		public float LifeTime { get; set; }
		public float TimePassed { get; set; }
	}
}