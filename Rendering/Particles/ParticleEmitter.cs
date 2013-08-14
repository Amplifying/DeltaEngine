using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Rendering.Particles
{
	/// <summary>
	///   Holds data on how to spawn particles and currently existant ones.
	///   Has Behaviors for drawing the particles and for spawning and removing them.
	/// </summary>
	public class ParticleEmitter : Entity2D
	{
		public ParticleEmitter(ParticleEmitterCreator emitterCreator, Point spawnPosition)
			: base(new Rectangle(spawnPosition, Size.Zero))
		{
			if (emitterCreator.ParticleMaterial == null)
				throw new UnableToCreateWithoutMaterial();
			Add(emitterCreator);
			Start<ParticleEmissionUpdate>();
			Start<SpawnUpdate>();
			OnDraw<ParticleRenderer>();
			if (emitterCreator.ParticleMaterial.Animation != null)
				Stop<UpdateImageAnimation>();
			if (emitterCreator.ParticleMaterial.SpriteSheet != null)
				Stop<UpdateSpriteSheetAnimation>();
			emitterCreator.StartColor = emitterCreator.ParticleMaterial.DefaultColor;
		}

		public class SpawnUpdate : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (ParticleEmitter emitter in entities)
					if (emitter.Get<ParticleEmitterCreator>().SpawnInterval != 0)
						emitter.SpawnNewParticles(emitter.Get<ParticleEmitterCreator>());
			}
		}

		public class UnableToCreateWithoutMaterial : Exception {}

		public class ParticleEmissionUpdate : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (ParticleEmitter emitter in entities)
					if (emitter.Get<ParticleEmitterCreator>().SpawnInterval != 0)
						emitter.UpdateEmitterAndParticles(emitter.Get<ParticleEmitterCreator>());
			}
		}

		private void UpdateEmitterAndParticles(ParticleEmitterCreator creator)
		{
			UpdateAndLimitNumberOfActiveParticles(creator);
			UpdateAnimation(creator);
		}

		private void UpdateAndLimitNumberOfActiveParticles(ParticleEmitterCreator creator)
		{
			int lastIndex = -1;
			for (int index = 0; index < NumberOfActiveParticles; index++)
				if (particles[index].UpdateIfStillActive(creator))
					lastIndex = index;
			NumberOfActiveParticles = lastIndex + 1;
		}

		private void UpdateAnimation(ParticleEmitterCreator creator)
		{
			if (Get<ParticleEmitterCreator>().ParticleMaterial.Animation != null)
				for (int index = 0; index < NumberOfActiveParticles; index++)
					UpdateAnimationForParticle(index, creator.ParticleMaterial);
			if (Get<ParticleEmitterCreator>().ParticleMaterial.SpriteSheet != null)
				for (int index = 0; index < NumberOfActiveParticles; index++)
					UpdateSpriteSheetAnimationForParticle(index, creator.ParticleMaterial);
		}

		private void UpdateAnimationForParticle(int index, Material material)
		{
			var animationData = material.Animation;
			particles[index].CurrentFrame =
				(int)(animationData.Frames.Length * particles[index].ElapsedTime / material.Duration) %
					animationData.Frames.Length;
			particles[index].Image = animationData.Frames[particles[index].CurrentFrame];
		}

		private void UpdateSpriteSheetAnimationForParticle(int index, Material material)
		{
			var animationData = material.SpriteSheet;
			particles[index].CurrentFrame =
				(int)(animationData.UVs.Count * particles[index].ElapsedTime / material.Duration) %
					animationData.UVs.Count;
			particles[index].CurrentUV = animationData.UVs[particles[index].CurrentFrame];
		}

		private void SpawnNewParticles(ParticleEmitterCreator creator)
		{
			if (particles == null || particles.Length != creator.MaximumNumberOfParticles)
				CreateParticlesArray(creator);
			ElapsedSinceLastSpawn += Time.Delta;
			while (ElapsedSinceLastSpawn >= creator.SpawnInterval)
				SpawnOneParticle(creator);
		}

		public Particle[] particles;
		public float ElapsedSinceLastSpawn { get; set; }

		public void CreateParticlesArray(ParticleEmitterCreator creator)
		{
			if (creator.MaximumNumberOfParticles > 512)
				throw new MaximumNumberOfParticlesExceeded(creator.MaximumNumberOfParticles, 512);
			particles = new Particle[creator.MaximumNumberOfParticles];
			Set(particles);
		}

		private class MaximumNumberOfParticlesExceeded : Exception
		{
			public MaximumNumberOfParticlesExceeded(int specified, int maxAllowed)
				: base("Specified=" + specified + ", Maximum allowed=" + maxAllowed) {}
		}

		private void SpawnOneParticle(ParticleEmitterCreator creator)
		{
			ElapsedSinceLastSpawn -= creator.SpawnInterval;
			int freeSpot = FindFreeSpot(creator);
			if (freeSpot < 0)
				return;
			particles[freeSpot].IsActive = true;
			particles[freeSpot].ElapsedTime = 0;
			particles[freeSpot].Position = Center;
			particles[freeSpot].SetStartVelocityRandomizedFromRange(creator.StartVelocity,
				creator.StartVelocityVariance);
			particles[freeSpot].Size = creator.Size.Start;
			particles[freeSpot].Color = creator.StartColor;
			particles[freeSpot].Image = creator.ParticleMaterial.DiffuseMap;
			particles[freeSpot].CurrentUV = creator.ParticleMaterial.SpriteSheet == null
				? Rectangle.One : creator.ParticleMaterial.SpriteSheet.UVs[0];
			particles[freeSpot].Rotation = creator.StartRotation;
		}

		private int FindFreeSpot(ParticleEmitterCreator creator)
		{
			for (int index = 0; index < NumberOfActiveParticles; index++)
				if (particles[index].ElapsedTime >= creator.LifeTime)
					return index;
			return NumberOfActiveParticles < creator.MaximumNumberOfParticles
				? NumberOfActiveParticles++ : -1;
		}

		public int NumberOfActiveParticles { get; set; }

		public void SpawnBurst(int numberOfParticles)
		{
			var data = Get<ParticleEmitterCreator>();
			if (particles == null || particles.Length != data.MaximumNumberOfParticles)
				CreateParticlesArray(data);
			for (int i = 0; i< numberOfParticles; i++)
				SpawnOneParticle(data);
		}

		public class ParticleRenderer : DrawBehavior
		{
			public ParticleRenderer(Drawing drawing)
			{
				this.drawing = drawing;
			}

			private readonly Drawing drawing;

			private class VerticesToRender
			{
				public VerticesToRender(Image texture, Material material)
				{
					Texture = texture;
					this.material = material;
					FillIndices(Data.Length);
				}

				public readonly Image Texture;
				public readonly Material material;

				private void FillIndices(int verticesCount)
				{
					for (int quad = 0; quad < verticesCount / 4; quad++)
					{
						Indices[quad * 6 + 0] = (short)(quad * 4 + 0);
						Indices[quad * 6 + 1] = (short)(quad * 4 + 1);
						Indices[quad * 6 + 2] = (short)(quad * 4 + 2);
						Indices[quad * 6 + 3] = (short)(quad * 4 + 0);
						Indices[quad * 6 + 4] = (short)(quad * 4 + 2);
						Indices[quad * 6 + 5] = (short)(quad * 4 + 3);
					}
				}

				public readonly VertexPosition2DColorUV[] Data = new VertexPosition2DColorUV[8 * 1024];
				public readonly short[] Indices = new short[12 * 1024];
				public int CurrentIndex;

				public void AddParticlesOfEmitter(Particle interpolatedParticle)
				{
					if (!interpolatedParticle.IsActive)
						return;
					if (CurrentIndex + 4 > Data.Length)
					{
						if (!alreadyWarned)
							Logger.Warning("Too many particles for " + interpolatedParticle.Image);
						alreadyWarned = true;
						return;
					}
					Data[CurrentIndex] = interpolatedParticle.GetTopLeftVertex();
					Data[CurrentIndex + 1] = interpolatedParticle.GetTopRightVertex();
					Data[CurrentIndex + 2] = interpolatedParticle.GetBottomRightVertex();
					Data[CurrentIndex + 3] = interpolatedParticle.GetBottomLeftVertex();
					CurrentIndex += 4;
				}

				private bool alreadyWarned;
			}

			public void Draw(IEnumerable<DrawableEntity> entities)
			{
				foreach (ParticleEmitter entity in entities)
					if (entity.NumberOfActiveParticles > 0)
						FillParticlesDrawContexts(entity);
				foreach (var verticesToRender in verticesPerTexture)
					if (verticesToRender.CurrentIndex > 0)
						RenderBatch(verticesToRender);
			}

			private readonly List<VerticesToRender> verticesPerTexture = new List<VerticesToRender>();

			private void FillParticlesDrawContexts(ParticleEmitter emitter)
			{
				foreach (var particle in emitter.GetInterpolatedArray<Particle>())
				{
					if (!particle.IsActive)
						continue;
					CreateVerticeToRenderIfNewImage(emitter, particle);
					foreach (var verticesToRender in verticesPerTexture)
						if (verticesToRender.Texture == particle.Image)
							verticesToRender.AddParticlesOfEmitter(particle);
				}
			}

			private void CreateVerticeToRenderIfNewImage(ParticleEmitter emitter, Particle particle)
			{
				bool hasVerticesWithImage = true;
				foreach (VerticesToRender x in verticesPerTexture)
					if (x.Texture == particle.Image)
						hasVerticesWithImage = false;
				if (hasVerticesWithImage)
					verticesPerTexture.Add(new VerticesToRender(particle.Image,
						emitter.Get<ParticleEmitterCreator>().ParticleMaterial));
			}

			private void RenderBatch(VerticesToRender vertices)
			{
				drawing.Add(vertices.material, vertices.Texture.BlendMode, vertices.Data, vertices.Indices,
					vertices.CurrentIndex, vertices.CurrentIndex * 6 / 4);
				vertices.CurrentIndex = 0;
			}
		}
	}
}