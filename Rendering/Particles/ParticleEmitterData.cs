using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Particles
{
	public class ParticleEmitterData
	{
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
	}
}
