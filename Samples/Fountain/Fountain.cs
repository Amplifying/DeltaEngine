using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering.Particles;

namespace FountainApp
{
	public class Fountain
	{
		public Fountain()
		{
			var image = ContentLoader.Load<Image>("Particle");
			FountainParticle = new ParticleEmitter(image);
			FountainParticle.Get<ParticleEmitterData>().Position = Point.Half - new Point(0.05f, 0.05f);
			FountainParticle.Get<ParticleEmitterData>().SpawnIntervalMs = 20;
			FountainParticle.Get<ParticleEmitterData>().PresetLowerBounds = RandomLowerPreset();
			FountainParticle.Get<ParticleEmitterData>().PresetUpperBounds = RandomUpperPreset();
		}

		public ParticleEmitter FountainParticle { get; set; }

		public ParticlePreset RandomLowerPreset()
		{
			var presetLowerBounds = new ParticlePreset
			{
				Position = new Point(-0.01f, -0.01f),
				StartVelocity = new Point(-0.2f, -1),
				Velocity = new Point(-0.2f, 2),
				StartRotation = 0,
				Rotation = 90,
				StartSize = new Size(0.01f, 0.01f),
				Size = new Size(0.015f, 0.015f),
				Lifetime = 1,
				Speed = 1,
				Color = Color.LightBlue
			};

			return presetLowerBounds;
		}

		public ParticlePreset RandomUpperPreset()
		{
			var presetUpperBounds = new ParticlePreset
			{
				Position = new Point(0.01f, 0.01f),
				StartVelocity = new Point(0.2f, -1),
				Velocity = new Point(0.2f, 2.5f),
				StartRotation = 90,
				Rotation = 180,
				StartSize = new Size(0.01f, 0.01f),
				Size = new Size(0.02f, 0.02f),
				Lifetime = 2,
				Speed = 3,
				Color = Color.Blue
			};

			return presetUpperBounds;
		}
	}
}
