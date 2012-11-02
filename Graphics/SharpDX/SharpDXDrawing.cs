using DeltaEngine.Datatypes;
using SharpDX;
using SharpDX.Direct2D1;
using Rectangle = DeltaEngine.Datatypes.Rectangle;

namespace DeltaEngine.Graphics.SharpDX
{
	/// <summary>
	/// Simple drawing support for DirectX 11, currently just allows to draw colored 2D rectangles.
	/// </summary>
	public class SharpDXDrawing : Drawing
	{
		public SharpDXDrawing(SharpDXDevice device)
			: base(device)
		{
			this.device = device;
			solidColorBrush = new SolidColorBrush(device.RenderTarget, Colors.Black);
			device.RenderTarget.Transform = Matrix3x2.Identity;
		}

		private readonly SharpDXDevice device;
		private readonly SolidColorBrush solidColorBrush;

		public override void SetColor(Color color)
		{
			if (color == lastColor)
				return;

			lastColor = color;
			solidColorBrush.Color = new Color4(color.PackedArgb);
		}

		private Color lastColor;

		public override void DrawRectangle(Rectangle area)
		{
			Rectangle pixelRect = device.Screen.ToPixelSpace(area);
			var sharpRect = new RectangleF(pixelRect.Left, pixelRect.Top, pixelRect.Right,
				pixelRect.Bottom);
			device.RenderTarget.FillRectangle(sharpRect, solidColorBrush);
		}
	}
}