using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D.Sprites;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class SpriteSheetPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			currentDisplayAnimation = new Sprite(new Material("Position2DUv", contentName),
				new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			SetImageCommands();
		}

		public Sprite currentDisplayAnimation;

		private void SetImageCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveImage).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => lastScalePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(ScaleImage).Add(new MousePositionTrigger(MouseButton.Middle, State.Pressed));
		}

		private Vector2D lastPanPosition = Vector2D.Unused;

		private void MoveImage(Vector2D mousePosition)
		{
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplayAnimation.Center += relativePosition;
		}

		private Vector2D lastScalePosition = Vector2D.Unused;

		private void ScaleImage(Vector2D mousePosition)
		{
			var relativePosition = mousePosition - lastScalePosition;
			lastScalePosition = mousePosition;
			currentDisplayAnimation.Size =
				new Size(
					currentDisplayAnimation.Size.Width +
						(currentDisplayAnimation.Size.Width * relativePosition.Y),
					currentDisplayAnimation.Size.Height +
						(currentDisplayAnimation.Size.Height * relativePosition.Y));
		}
	}
}