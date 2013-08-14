using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Platforms.Mocks;
using DeltaEngine.Rendering.Particles;
using DeltaEngine.Rendering.Tests.Sprites;
using NUnit.Framework;

namespace DeltaEngine.Rendering.Tests.Particles
{
	internal class ParticleEmitterTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateEmitterAndKeepRunning()
		{
			CreateDataAndEmitter(512, 0.01f, 5);
			emitter.Center = new Point(0.5f, 0.7f);
			new PerformanceTests.FpsDisplay();
		}

		private void CreateDataAndEmitter(int maxParticles = 1, float spawnInterval = 0.01f,
			float lifeTime = 0.2f)
		{
			emitterData = new ParticleEmitterCreator
			{
				MaximumNumberOfParticles = maxParticles,
				SpawnInterval = spawnInterval,
				LifeTime = lifeTime,
				Size = new Range<Size>(new Size(0.05f), new Size(0.02f)),
				StartColor = Color.Orange,
				Force = new Point(0, 0.1f),
				StartVelocity = new Point(0, -0.3f),
				StartVelocityVariance = new Point(0.05f, 0.01f),
				StartRotation = 0.2f,
				ParticleMaterial = new Material(Shader.Position2DColorUv, "DeltaEngineLogo")
			};
			emitter = new ParticleEmitter(emitterData, Point.Half);
		}

		private ParticleEmitterCreator emitterData;
		private ParticleEmitter emitter;

		[Test]
		public void CreateEmitterWithJustOneParticle()
		{
			CreateDataAndEmitter(1, 0.01f, 5);
			emitter.Center = new Point(0.5f, 0.7f);
			RunAfterFirstFrame(() =>
			{
				//Assert.AreEqual(1, Resolve<Drawing>().NumberOfDynamicDrawCallsThisFrame);
				//Assert.AreEqual(4, Resolve<Drawing>().NumberOfDynamicVerticesDrawnThisFrame);
			});
		}

		[Test, CloseAfterFirstFrame]
		public void AdvanceCreatingOneParticle()
		{
			CreateDataAndEmitter();
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(emitter.NumberOfActiveParticles, 1);
		}

		[Test]
		public void MovingTheEmitterMovesRenderedParticlesRelatively()
		{
			CreateDataAndEmitter(10);
			emitter.Start<Transition>().Add(new Transition.Position(Point.Zero, Point.One)).Add(
				new Transition.Duration(5.0f));
		}

		[Test]
		public void RotatingEmitterMovesParticlesRelatively()
		{
			CreateDataAndEmitter(500, 0.05f, 2);
			emitter.Start<Transition>().Add(new Transition.Rotation(0.0f, 720.0f)).Add(
				new Transition.Duration(5.0f));
		}

		[Test]
		public void MultipleEmittersShallNotInterfere()
		{
			CreateDataAndEmitter(12, 0.1f, 5);
			var data = new ParticleEmitterCreator
			{
				MaximumNumberOfParticles = 12,
				SpawnInterval = 0.4f,
				LifeTime = 2,
				Size = new Range<Size>(new Size(0.03f), new Size(0.03f)),
				StartColor = Color.Gray,
				Force = -Point.UnitY,
				StartVelocity = new Point(0.3f, -0.1f),
				ParticleMaterial = new Material(Shader.Position2DColorUv, "DeltaEngineLogo")
			};
			new ParticleEmitter(data, Point.Half);
		}

		[Test, CloseAfterFirstFrame]
		public void ParticlesUpdatingPosition()
		{
			CreateDataAndEmitter();
			if (resolver.GetType() == typeof(MockResolver))
				AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreNotEqual(emitter.Center, emitter.particles[0].Position);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateParticleEmitterAddingDefaultComponents()
		{
			var emptyMaterial = new Material(Shader.Position2DColor, "");
			emitter = new ParticleEmitter(new ParticleEmitterCreator { ParticleMaterial = emptyMaterial },
				Point.Zero);
			Assert.NotNull(emitter.Get<ParticleEmitterCreator>());
		}

		[Test]
		public void CreateEmitterAndKeepRunningWithAnimation()
		{
			CreateDataAndEmitterWithAnimation("ImageAnimation");
			emitter.Center = new Point(0.5f, 0.7f);
			new PerformanceTests.FpsDisplay();
		}

		private ParticleEmitterCreator CreateDataAndEmitterWithAnimation(string contentName)
		{
			emitterData = new ParticleEmitterCreator
			{
				MaximumNumberOfParticles = 512,
				SpawnInterval = 0.01f,
				LifeTime = 5f,
				Size = new Range<Size>(new Size(0.05f), new Size(0.10f)),
				StartColor = Color.White,
				Force = new Point(0, 0.1f),
				StartVelocity = new Point(0, -0.3f),
				StartVelocityVariance = new Point(0.05f, 0.01f),
				ParticleMaterial = new Material(Shader.Position2DColorUv, contentName)
			};
			return emitterData;
		}

		[Test]
		public void CreateEmitterAndKeepRunningWithSpriteSheetAnimation()
		{
			emitterData = CreateDataAndEmitterWithAnimation("SpriteSheetAnimation");
			emitter = new ParticleEmitter(emitterData, Point.Half);
			emitter.Center = new Point(0.5f, 0.7f);
			new PerformanceTests.FpsDisplay();
		}

		[Test]
		public void CreatRotatedParticles()
		{
			emitterData = CreateDataAndEmitterWithAnimation("DeltaEngineLogo");
			emitterData.StartRotation = 45;
			emitterData.Size = new Range<Size>(new Size(0.05f), new Size(0.05f));
			emitter = new ParticleEmitter(emitterData, Point.Half);
			emitter.Center = new Point(0.5f, 0.7f);
			new PerformanceTests.FpsDisplay();
		}

		[Test]
		public void CreatRotatingParticles()
		{
			emitterData = CreateDataAndEmitterWithAnimation("DeltaEngineLogo");
			emitterData.StartRotation = 45;
			emitterData.RotationForce = 1;
			emitterData.Size = new Range<Size>(new Size(0.05f), new Size(0.05f));
			emitter = new ParticleEmitter(emitterData, Point.Half);
			emitter.Center = new Point(0.5f, 0.7f);
			new PerformanceTests.FpsDisplay();
		}

		[Test]
		public void SpawnBurst()
		{
			CreateDataAndEmitter(500);
			emitterData.LifeTime = 10;
			emitter.Stop<ParticleEmitter.SpawnUpdate>();
			//emitter.SpawnBurst(20);
			emitter.Start<BurstUpdate>();
		}

		private class BurstUpdate: UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					if (Time.CheckEvery(3))
					{
						var emitter = entity as ParticleEmitter;
						emitter.SpawnBurst(10);
					}
				}
			}
		}
	}
}