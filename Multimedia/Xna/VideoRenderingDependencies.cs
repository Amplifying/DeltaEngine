using System;
using DeltaEngine.Entities;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Rendering;
using Microsoft.Xna.Framework.Content;

namespace DeltaEngine.Multimedia.Xna
{
	public class VideoRenderingDependencies
	{
		public VideoRenderingDependencies(XnaDrawing drawing, XnaDevice graphicsDevice,
			EntitySystem entitySystem, ScreenSpace screen)
		{
			if (graphicsDevice == null || graphicsDevice.NativeContent == null)
				throw new UnableToContinueWithoutXnaGraphicsDevice();

			Drawing = drawing;
			GraphicsDevice = graphicsDevice;
			EntitySystem = entitySystem;
			Screen = screen;
		}

		public readonly XnaDrawing Drawing;
		public readonly XnaDevice GraphicsDevice;
		public readonly EntitySystem EntitySystem;
		public readonly ScreenSpace Screen;
		public ContentManager NativeContent
		{
			get { return GraphicsDevice.NativeContent; }
		}

		internal class UnableToContinueWithoutXnaGraphicsDevice : Exception { }
	}
}
