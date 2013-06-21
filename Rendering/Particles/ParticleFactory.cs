using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Rendering.Particles
{
	internal class ParticleFactory : EntityHandler
	{
		public override void Handle(Entity entity)
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
				CreateParticle(GetRandomizedPreset(), data);
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

		private void CreateParticle(ParticlePreset preset, ParticleEmitterData data)
		{
			var drawArea = new Rectangle(data.Position + preset.Position, preset.Size);
			var particle = new Sprite(data.Image, drawArea);
			data.ParticlesCreated++;
			particle.Add<ParticleUpdater>();
			particle.Add(new Transition.Duration(preset.Lifetime));
			particle.Add(new Transition.TransitionEnded());
			particle.Add(new Transition.FadingColor(preset.Color));
			particle.Add(new ParticleInfo(preset.StartVelocity, preset.Velocity, preset.Speed,
				preset.Lifetime));
			SetPosibleTransition(preset, particle);
		}

		private void SetPosibleTransition(ParticlePreset preset, Sprite particle)
		{
			if (!data.HasSecondaryDirection)
				particle.Get<ParticleInfo>().Direction = particle.Get<ParticleInfo>().StartDirection;
			particle.Add(new Transition.Position(particle.Center, SetEndPosition(preset)));
			if (data.HasSecondaryRotation)
				particle.Add(new Transition.Rotation(preset.StartRotation, preset.Rotation));
			if (data.HasSecondarySize)
				particle.Add(new Transition.Size(preset.StartSize, preset.Size));
		}

		private Point SetEndPosition(ParticlePreset preset)
		{
			Point endPos = data.Position + ((preset.Speed * preset.StartVelocity) * preset.Lifetime);
			return endPos;
		}
	}
}