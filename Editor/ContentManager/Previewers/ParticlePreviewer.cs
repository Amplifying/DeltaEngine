using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Particles;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ParticlePreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var particleEmitterData = ContentLoader.Load<ParticleEmitterData>(contentName);
			particleEmitterData.StartPosition = new RangeGraph<Vector3D>(new Vector2D(0.25f, 0.25f),
				new Vector2D(0.25f, 0.25f));
			currentDisplayParticle2D = new ParticleEmitter(particleEmitterData, Vector2D.Half);
			SetImageCommands();
		}

		public ParticleEmitter currentDisplayParticle2D;

		private void SetImageCommands()
		{
			ContentDisplayChanger.SetParticleDMoveCommand(currentDisplayParticle2D);
		}
	}
}