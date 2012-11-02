using DeltaEngine.Datatypes;
using Microsoft.Xna.Framework.Graphics;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace DeltaEngine.Graphics.Xna
{
	public class XnaDrawing : Drawing
	{
		public XnaDrawing(XnaDevice device)
			: base(device)
		{
			this.device = device;
		}

		private readonly XnaDevice device;

		public override void SetColor(Color color)
		{
			if (brushTexture == null)
				brushTexture = new Texture2D(device.NativeDevice, 1, 1);

			if (color == lastColor)
				return;

			lastColor = color;
			MakeSureTextureIsUnsetFromDevice();
			brushTexture.SetData(new[] { color });
		}

		private void MakeSureTextureIsUnsetFromDevice()
		{
			device.NativeDevice.Textures[0] = null;
		}

		private Color lastColor;
		private SpriteBatch spriteBatch;
		private Texture2D brushTexture;

		public override void DrawRectangle(Rectangle area)
		{
			if (spriteBatch == null)
				spriteBatch = new SpriteBatch(device.NativeDevice);

			spriteBatch.Begin();
			spriteBatch.Draw(brushTexture, ConvertToNativeXnaRectangle(area), XnaColor.White);
			spriteBatch.End();
		}
		
		private XnaRectangle ConvertToNativeXnaRectangle(Rectangle rect)
		{
			Rectangle pixelRect = device.Screen.ToPixelSpace(rect);
			return new XnaRectangle((int)pixelRect.Left, (int)pixelRect.Top, (int)pixelRect.Width,
				(int)pixelRect.Height);
		}
	}
}