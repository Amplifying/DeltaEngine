using System;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Rendering.ScreenSpaces;
using Microsoft.Xna.Framework.Content;

namespace DeltaEngine.Multimedia.Xna
{
	public class VideoRenderingDependencies
	{
		public VideoRenderingDependencies(XnaDevice graphicsDevice, ScreenSpace screen)
		{
			if (graphicsDevice == null || graphicsDevice.NativeContent == null)
				throw new UnableToContinueWithoutXnaGraphicsDevice();

			GraphicsDevice = graphicsDevice;
			Screen = screen;
		}

		public readonly XnaDevice GraphicsDevice;
		public readonly ScreenSpace Screen;
		public ContentManager NativeContent
		{
			get { return GraphicsDevice.NativeContent; }
		}

		internal class UnableToContinueWithoutXnaGraphicsDevice : Exception {}
	}
}