using System;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Rendering;
using Microsoft.Xna.Framework.Graphics;

namespace DeltaEngine.Multimedia.Xna
{
	public class XnaVideoSurface : VideoSurface
	{
		public XnaVideoSurface(Video video, Drawing drawing, Renderer renderer,
			XnaDevice device, XnaSoundDevice soundDevice)
			: base(drawing, renderer, video)
		{
			nativeDevice = device.NativeDevice;
			this.soundDevice = soundDevice;
		}

		private readonly GraphicsDevice nativeDevice;
		private readonly XnaSoundDevice soundDevice;

		protected override void Render(Renderer renderer, Core.Time time)
		{
			try
			{
				nativeDevice.Textures[0] = soundDevice.NativePlayer.GetTexture();
				nativeDevice.SamplerStates[0] = SamplerState.LinearClamp;
			}
			catch (Exception)
			{
			}
			base.Render(renderer, time);
		}
	}
}