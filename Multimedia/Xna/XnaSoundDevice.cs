using DeltaEngine.Graphics.Xna;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native implementation of a SoundDevice using xna that calls the FrameworkDispatcher.
	/// </summary>
	public class XnaSoundDevice : SoundDevice
	{
		public XnaSoundDevice(XnaDevice graphicsDevice)
		{
			Content = graphicsDevice.NativeContent;
			NativePlayer = new VideoPlayer();
		}

		public ContentManager Content { get; private set; }

		public VideoPlayer NativePlayer { get; private set; }

		public override void Dispose()
		{
			if (NativePlayer != null)
				NativePlayer.Dispose();

			NativePlayer = null;
			Content = null;
		}
	}
}