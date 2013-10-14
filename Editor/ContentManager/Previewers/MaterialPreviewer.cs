using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D;

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

		private void SetBillboardCommands()
		{
			new Command(position => lastPanPosition = position).Add(new MouseButtonTrigger());
			new Command(MoveBillboard).Add(new MousePositionTrigger(MouseButton.Left, State.Pressed));
		}

		private Vector2D lastPanPosition = Vector2D.Unused;

		private void MoveBillboard(Vector2D mousePosition)
		{
			var relativePosition = mousePosition - lastPanPosition;
			lastPanPosition = mousePosition;
			currentDisplayBillboard.Position -= relativePosition;
		}

		private void SetImageCommands()
		{
			ContentDisplayChanger.SetEntity2DMoveCommand(currentDisplaySprite);
			ContentDisplayChanger.SetEntity2DScaleCommand(currentDisplaySprite);
		}
	}
}