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
				Particle3DPointEmitter = new Particle3DPointEmitter(particleEmitterData, Vector2D.Half);
			else
				currentDisplayParticle2D = new Particle2DEmitter(particleEmitterData, Vector2D.Half);
			SetImageCommands2D();
			SetImageCommands3D();
		}

		public Particle2DEmitter currentDisplayParticle2D;
		public Particle3DPointEmitter Particle3DPointEmitter;

		private void SetImageCommands2D()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveImage).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
		}

		private Vector2D lastPanPosition = Vector2D.Unused;

		private void MoveImage(Vector2D mousePosition)
		{
			if (currentDisplayParticle2D == null)
				return;
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplayParticle2D.Position += relativePosition;
		}

		private void SetImageCommands3D()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveImage3D).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
		}

		private void MoveImage3D(Vector2D mousePosition)
		{
			if (Particle3DPointEmitter == null)
				return;
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			Particle3DPointEmitter.Position += relativePosition;
		}
	}
}