using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public sealed class ImageAnimationPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			currentDisplayAnimation =
				new Sprite(
					new Material(Shader.Position2DUv, contentName), new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			SetImageCommands();
		}

		private Sprite currentDisplayAnimation;

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
			if (relativePosition == Point.Zero)
				return;
			currentDisplayAnimation.Center += relativePosition;
		}

		private Point lastScalePosition = Point.Unused;

		private void ScaleImage(Point mousePosition)
		{
			var relativePosition = mousePosition - lastScalePosition;
			lastScalePosition = mousePosition;
			if (relativePosition == Point.Zero)
				return;
			currentDisplayAnimation.Size =
				new Size(
					currentDisplayAnimation.Size.Width +
						(currentDisplayAnimation.Size.Width * relativePosition.Y),
					currentDisplayAnimation.Size.Height +
						(currentDisplayAnimation.Size.Height * relativePosition.Y));
		}
	}
}