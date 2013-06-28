using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Particles
{
	public class ParticleEmitter : Entity2D
	{
		public ParticleEmitter(Image image)
			: base(Rectangle.Zero)
		{
			Start<ParticleFactory>();
			Add(new ParticleEmitterData
			{
				Image = image,
				Position = Point.Half,
				SpawnIntervalMs = DefaultSpawnIntervalMs,
				LastSpawnMs = NeverSpawned,
			});
		}

		private const int DefaultSpawnIntervalMs = 5;
		public const long NeverSpawned = -1;

		public int ParticlesCreated
		{
			get { return Get<ParticleEmitterData>().ParticlesCreated; }
		}
	}
}
