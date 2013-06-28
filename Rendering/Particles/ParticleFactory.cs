using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Rendering.Particles
{
	internal class ParticleFactory : Behavior2D
	{
		public override void Handle(Entity2D entity)
		{
			Create((ParticleEmitter)entity);
		}

		private void Create(ParticleEmitter emitter)
		{
			data = emitter.Get<ParticleEmitterData>();
			int particlesToSpawn = GetNumberOfParticleToSpawn();
			if (particlesToSpawn == 0)
				return;

			for (int i = 0; i < particlesToSpawn; i++)
				CreateParticle(GetRandomizedPreset());
			data.LastSpawnMs = Time.Current.Milliseconds;
		}

		private ParticleEmitterData data;

		private int GetNumberOfParticleToSpawn()
		{
			if (HasNeverSpawnedYet)
				return 1;

			return TimeSinceLastSpawn / data.SpawnIntervalMs;
		}

		private bool HasNeverSpawnedYet
		{
			get { return data.LastSpawnMs == ParticleEmitter.NeverSpawned; }
		}

		private int TimeSinceLastSpawn
		{
			get { return (int)(Time.Current.Milliseconds - data.LastSpawnMs); }
		}

		private ParticlePreset GetRandomizedPreset()
		{
			return new ParticlePreset(data.PresetLowerBounds, data.PresetUpperBounds);
		}

		private void CreateParticle(ParticlePreset preset)
		{
			var drawArea = new Rectangle(data.Position + preset.Position, preset.Size);
			var particle = new Sprite(data.Image, drawArea);
			data.ParticlesCreated++;
			particle.Start<ParticleUpdater>();
			particle.Add(new Transition.Duration(preset.Lifetime));
			particle.Add(new Transition.TransitionEnded());
			particle.Add(new Transition.FadingColor(preset.Color));
			particle.Add(new ParticleInfo(preset.StartVelocity, preset.Velocity, preset.Speed,
				preset.Lifetime));
			particle.Add(new Transition.Position(particle.Center, SetEndPosition(preset)));
			particle.Add(new Transition.Rotation(preset.StartRotation, preset.Rotation));
			particle.Add(new Transition.Size(preset.StartSize, preset.Size));
		}

		private Point SetEndPosition(ParticlePreset preset)
		{
			Point endPos = data.Position + ((preset.Speed * preset.StartVelocity) * preset.Lifetime);
			return endPos;
		}
	}
}