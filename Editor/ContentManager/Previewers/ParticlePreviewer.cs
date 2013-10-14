using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering3D.Particles;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ParticlePreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var particleEmitterData = ContentLoader.Load<ParticleEmitterData>(contentName);
			particleEmitterData.StartPosition = new RangeGraph<Vector3D>(new Vector2D(0.25f, 0.25f),
				new Vector2D(0.25f, 0.25f));
			var shaderWithFormat = particleEmitterData.ParticleMaterial.Shader as ShaderWithFormat;
			if (shaderWithFormat.Format.Is3D)
				Particle3DPointEmitter = ResetEmitter3D(particleEmitterData);
			else
				currentDisplayParticle2D = new ParticleEmitter(particleEmitterData, Vector2D.Half);
			SetImageCommands();
		}

		public ParticleEmitter currentDisplayParticle2D;
		public ParticleEmitter Particle3DPointEmitter;

		private static ParticleEmitter ResetEmitter3D(ParticleEmitterData emitterData)
		{
			if (emitterData.EmitterType == "PointEmitter")
				return new Particle3DPointEmitter(emitterData, emitterData.StartPosition.Start);
			if (emitterData.EmitterType == "LineEmitter")
				return new Particle3DLineEmitter(emitterData, emitterData.StartPosition);
			if (emitterData.EmitterType == "BoxEmitter")
				return new Particle3DBoxEmitter(emitterData, emitterData.StartPosition);
			return new Particle3DSphericalEmitter(emitterData, emitterData.StartPosition.Start,
					emitterData.StartPosition.End.Length);
		}

		private void SetImageCommands()
		{
			ContentDisplayChanger.SetParticleDMoveCommand(Particle3DPointEmitter);
			ContentDisplayChanger.SetParticleDMoveCommand(currentDisplayParticle2D);
		}
	}
}