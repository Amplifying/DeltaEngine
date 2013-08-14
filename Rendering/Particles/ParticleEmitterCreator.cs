using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Particles
{
	/// <summary>
	/// Data for ParticleEmitter, usually created and saved in the Editor.
	/// </summary>
	public class ParticleEmitterCreator : ContentData
	{
		public ParticleEmitterCreator()
			: base("<GeneratedParticleEmitterData>") { }

		public ParticleEmitterCreator(string contentName)
			: base(contentName) {}

		protected override void DisposeData(){}

		public float SpawnInterval { get; set; }
		public float LifeTime { get; set; }
		public int MaximumNumberOfParticles { get; set; }
		public Point StartVelocity { get; set; }
		public Point StartVelocityVariance { get; set; }
		public Point Force { get; set; }
		public Range<Size> Size { get; set; }
		public float StartRotation { get; set; }
		public float RotationForce { get; set; }
		public Color StartColor { get; set; }
		public Material ParticleMaterial { get; set; }

		protected override void LoadData(Stream fileData)
		{
			var emitterData = (ParticleEmitterData)new BinaryReader(fileData).Create();
			SpawnInterval = emitterData.SpawnInterval;
			LifeTime = emitterData.LifeTime;
			MaximumNumberOfParticles = emitterData.MaximumNumberOfParticles;
			StartVelocity = emitterData.StartVelocity;
			StartVelocityVariance = emitterData.StartVelocityVariance;
			Force = emitterData.Force;
			Size = emitterData.Size;
			StartRotation = emitterData.StartRotation;
			StartColor = emitterData.ParticleMaterial.DefaultColor;
			ParticleMaterial = emitterData.ParticleMaterial;
		}
	}
}