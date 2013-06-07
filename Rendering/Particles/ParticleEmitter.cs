using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering.Particles
{
	public class ParticleEmitter : Entity
	{
		public ParticleEmitter(Image image)
		{
			Add<ParticleFactory>();
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
