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
			particleEmitterData.StartPosition = new RangeGraph<Point>(new Point(0.25f, 0.25f),
				new Point(0.25f, 0.25f));
			currentDisplayParticle = new ParticleEmitter(particleEmitterData, Point.Half);
			SetImageCommands();
		}

		public ParticleEmitter currentDisplayParticle;

		private void SetImageCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveImage).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => lastScalePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(ScaleImage).Add(new MousePositionTrigger(MouseButton.Middle, State.Pressed));
		}

		private Point lastPanPosition = Point.Unused;

		private void MoveImage(Point mousePosition)
		{
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplayParticle.Center += relativePosition;
		}

		private Point lastScalePosition = Point.Unused;

		private void ScaleImage(Point mousePosition)
		{
			var relativePosition = mousePosition - lastScalePosition;
			lastScalePosition = mousePosition;
			currentDisplayParticle.Size =
				new Size(
					currentDisplayParticle.Size.Width +
						(currentDisplayParticle.Size.Width * relativePosition.Y),
					currentDisplayParticle.Size.Height +
						(currentDisplayParticle.Size.Height * relativePosition.Y));
		}
	}
}