using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms.All;
using DeltaEngine.Platforms.Tests;
using DeltaEngine.Rendering.Particles;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Particles
{
	internal class ParticelTests : TestWithAllFrameworks
	{
		[VisualTest]
		public void CreatedstaticParticles(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var image = content.Load<Image>("DeltaEngineLogo");
				var particleEmitter = new ParticleEmitter(image);
				particleEmitter.Get<ParticleEmitterData>().Position = Point.Half - new Point(0.05f, 0.05f);
				particleEmitter.Get<ParticleEmitterData>().PresetLowerBounds = BasicLowerPreset();
				particleEmitter.Get<ParticleEmitterData>().PresetUpperBounds = BasicUpperPreset();
			});
		}

		public ParticlePreset BasicLowerPreset()
		{
			var presetLowerBounds = new ParticlePreset
			{
				Position = new Point(0, 0),
				StartVelocity = new Point(0, 0),
				Velocity = new Point(0, 0),
				StartRotation = 0,
				Rotation = 0,
				StartSize = new Size(0.1f, 0.1f),
				Size = new Size(0.1f, 0.1f),
				Lifetime = 1,
				Color = Color.White
			};

			return presetLowerBounds;
		}

		public ParticlePreset BasicUpperPreset()
		{
			var emptyUpperPreset = new ParticlePreset
			{
				Position = new Point(0, 0),
				StartVelocity = new Point(0, 0),
				Velocity = new Point(0, 0),
				StartRotation = 0,
				Rotation = 0,
				StartSize = new Size(0.1f, 0.1f),
				Size = new Size(0.1f, 0.1f),
				Lifetime = 1,
				Color = Color.White
			};

			return emptyUpperPreset;
		}

		[VisualTest]
		public void DeleteParticlesAfterLifeTime(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var emitter = CreateBasicParticleEmitter(content);
				emitter.Get<ParticleEmitterData>().PresetLowerBounds.Lifetime = 0.5f;
				emitter.Get<ParticleEmitterData>().PresetUpperBounds.Lifetime = 0.5f;
			});
		}

		private ParticleEmitter CreateBasicParticleEmitter(ContentLoader content)
		{
			var image = content.Load<Image>("DeltaEngineLogo");
			var particleEmitter = new ParticleEmitter(image);
			particleEmitter.Get<ParticleEmitterData>().Position = Point.Half - new Point(0.05f, 0.05f);
			particleEmitter.Get<ParticleEmitterData>().SpawnIntervalMs = 1000;
			particleEmitter.Get<ParticleEmitterData>().PresetLowerBounds = BasicLowerPreset();
			particleEmitter.Get<ParticleEmitterData>().PresetUpperBounds = BasicUpperPreset();
			return particleEmitter;
		}

		[VisualTest]
		public void ParticlesWithVelecityAndSpeed(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var particleEmitter = CreateBasicParticleEmitter(content);
				SetUpVelocityAndSpeed(particleEmitter);
			});
		}

		private static void SetUpVelocityAndSpeed(ParticleEmitter particleEmitter)
		{
			particleEmitter.Get<ParticleEmitterData>().PresetLowerBounds.Speed = 1;
			particleEmitter.Get<ParticleEmitterData>().PresetUpperBounds.Speed = 1;
			particleEmitter.Get<ParticleEmitterData>().PresetLowerBounds.StartVelocity = new Point(-1, -1);
			particleEmitter.Get<ParticleEmitterData>().PresetUpperBounds.StartVelocity = new Point(-1, -1);
		}

		[VisualTest]
		public void ChangingVelocity(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var particleEmitter = CreateBasicParticleEmitter(content);
				SetUpVelocityAndSpeed(particleEmitter);
				particleEmitter.Get<ParticleEmitterData>().PresetLowerBounds.Velocity = new Point(-1, 1);
				particleEmitter.Get<ParticleEmitterData>().PresetUpperBounds.Velocity = new Point(-1, 1);
			});
		}

		[VisualTest]
		public void ChangeSizeAndRotateParticle(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var particleEmitter = CreateBasicParticleEmitter(content);
				SetUpVelocityAndSpeed(particleEmitter);
				particleEmitter.Get<ParticleEmitterData>().PresetLowerBounds.Size = new Size(0.2f, 0.2f);
				particleEmitter.Get<ParticleEmitterData>().PresetUpperBounds.Size = new Size(0.2f, 0.2f);
				particleEmitter.Get<ParticleEmitterData>().PresetLowerBounds.Rotation = 360;
				particleEmitter.Get<ParticleEmitterData>().PresetUpperBounds.Rotation = 360;
			});
		}

		[VisualTest]
		public void GiveRandomValues(Type resolver)
		{
			Start(resolver, (ContentLoader content) =>
			{
				var particleEmitter = CreateRandomParticleEmitter(content);
			});
		}

		private ParticleEmitter CreateRandomParticleEmitter(ContentLoader content)
		{
			var image = content.Load<Image>("DeltaEngineLogo");
			var particleEmitter = new ParticleEmitter(image);
			particleEmitter.Get<ParticleEmitterData>().Position = Point.Half - new Point(0.05f, 0.05f);
			particleEmitter.Get<ParticleEmitterData>().SpawnIntervalMs = 20;
			particleEmitter.Get<ParticleEmitterData>().PresetLowerBounds = RandomLowerPreset();
			particleEmitter.Get<ParticleEmitterData>().PresetUpperBounds = RandomUpperPreset();
			return particleEmitter;
		}

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
			var emptyUpperPreset = new ParticlePreset
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

			return emptyUpperPreset;
		}

		[Test, Category("Slow")]
		public void TestParticleHandler()
		{
			Start(typeof(MockResolver), (ContentLoader content) =>
			{ 
				var particleEmitter = CreateRandomParticleEmitter(content); 
				mockResolver.AdvanceTimeAndExecuteRunners(1.5f);
				Assert.AreNotEqual(0, particleEmitter.ParticlesCreated);
			});
		}
	}
}