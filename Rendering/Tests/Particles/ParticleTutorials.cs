using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Platforms.All;
using DeltaEngine.Rendering.Particles;

namespace DeltaEngine.Rendering.Tests.Particles
{
	internal class ParticelTutorials : TestWithAllFrameworks
	{
		[VisualTest]
		public void CreatedLensFlareEffect(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var particleEmitter = new ParticleEmitter(image);
				particleEmitter.Get<ParticleEmitterData>().Position = Point.Half - new Point(0.05f, 0.05f);
				particleEmitter.Get<ParticleEmitterData>().PresetLowerBounds = CreateBasicLowerPreset();
				particleEmitter.Get<ParticleEmitterData>().PresetUpperBounds = CreateBasicUpperPreset();
				particleEmitter.Get<ParticleEmitterData>().HasSecondaryDirection = false;
				particleEmitter.Get<ParticleEmitterData>().HasSecondaryRotation = false;
				particleEmitter.Get<ParticleEmitterData>().HasSecondarySize = false;
				particleEmitter.Add<MoveToMousePosition>();
			});
		}

		public class MoveToMousePosition : EntityHandler
		{
			public MoveToMousePosition(Mouse mouse)
			{
				this.mouse = mouse;
			}

			private readonly Mouse mouse;

			public override void Handle(Entity entity)
			{
				if (entity.GetType() == typeof(ParticleEmitter))
					entity.Get<ParticleEmitterData>().Position = mouse.Position;
			}
		}

		public ParticlePreset CreateBasicLowerPreset()
		{
			return new ParticlePreset
			{
				Position = new Point(0, 0),
				Speed = 0.5f,
				StartVelocity = new Point(-1, -1),
				Velocity = new Point(0, 0),
				StartRotation = 0,
				Rotation = 0,
				StartSize = new Size(0.1f, 0.1f),
				Size = new Size(0.1f, 0.1f),
				Lifetime = 1,
				Color = Color.White
			};
		}

		public ParticlePreset CreateBasicUpperPreset()
		{
			return new ParticlePreset
			{
				Position = new Point(0, 0),
				Speed = 1,
				StartVelocity = new Point(1, 1),
				Velocity = new Point(0, 0),
				StartRotation = 0,
				Rotation = 0,
				StartSize = new Size(0.1f, 0.1f),
				Size = new Size(0.1f, 0.1f),
				Lifetime = 1,
				Color = Color.White
			};
		}
	}
}