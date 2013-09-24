using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D.Sprites;
using DeltaEngine.Rendering3D.Models;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class MaterialPreviewer : ContentPreview
	{
		public void PreviewContent(string contentName)
		{
			var material = ContentLoader.Load<Material>(contentName);
			var imageSize = material.DiffuseMap.PixelSize;
			var aspectRatio = imageSize.Height / imageSize.Width;
			var shaderWithFormat = material.Shader as ShaderWithFormat;
			if (shaderWithFormat.Format.Is3D)
			{
				currentDisplayBillboard = new Billboard(Vector3D.Zero, Size.One, material);
				SetBillboardCommands();
			}
			else
			{
				currentDisplaySprite = new Sprite(material,
					Rectangle.FromCenter(new Vector2D(0.5f, 0.5f), new Size(0.5f, 0.5f * aspectRatio)));
				SetImageCommands();
			}
		}

		public Sprite currentDisplaySprite;
		private Billboard currentDisplayBillboard;

		private void SetImageCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveImage).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => lastScalePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
			new Command(ScaleImage).Add(new MousePositionTrigger(MouseButton.Middle, State.Pressed));
		}

		private void MoveImage(Vector2D mousePosition)
		{
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplaySprite.Center += relativePosition;
		}

		private void ScaleImage(Vector2D mousePosition)
		{
			var relativePosition = mousePosition - lastScalePosition;
			lastScalePosition = mousePosition;

			currentDisplaySprite.Size =
				new Size(
					currentDisplaySprite.Size.Width + (currentDisplaySprite.Size.Width * relativePosition.Y),
					currentDisplaySprite.Size.Height + (currentDisplaySprite.Size.Height * relativePosition.Y));
		}

		private void SetBillboardCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveBillboard).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
			new Command(position => lastScalePosition = position).Add(
				new MouseButtonTrigger(MouseButton.Middle));
		}

		private Vector2D lastPanPosition = Vector2D.Unused;

		private void MoveBillboard(Vector2D mousePosition)
		{
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplayBillboard.Position -= relativePosition;
		}

		private Vector2D lastScalePosition = Vector2D.Unused;
	}
}