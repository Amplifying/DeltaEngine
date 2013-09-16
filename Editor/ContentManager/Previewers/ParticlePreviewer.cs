using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Particles;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ParticlePreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var particleEmitterData = ContentLoader.Load<ParticleEmitterData>(contentName);
			particleEmitterData.StartPosition = new RangeGraph<Vector>(new Point(0.25f, 0.25f),
				new Point(0.25f, 0.25f));
			currentDisplayParticle = new Particle2DEmitter(particleEmitterData, Point.Half);
			SetImageCommands();
		}

		public Particle2DEmitter currentDisplayParticle;

		private void SetImageCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveImage).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
		}

		private Point lastPanPosition = Point.Unused;

		private void MoveImage(Point mousePosition)
		{
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplayParticle.Position += relativePosition;
		}
	}
}