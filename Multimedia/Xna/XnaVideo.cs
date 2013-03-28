using System;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Xna;
using DeltaEngine.Rendering;
using Media = Microsoft.Xna.Framework.Media;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native Xna implementation for video playback.
	/// </summary>
	public class XnaVideo : Video
	{
		public XnaVideo(string filename, Drawing drawing, Renderer renderer, XnaSoundDevice device,
			XnaDevice graphicsDevice)
			: base(filename, renderer)
		{
			this.drawing = drawing;
			this.device = device;
			this.graphicsDevice = graphicsDevice;
			if (graphicsDevice == null || device.Content == null)
				throw new UnableToContinueWithoutXnaGraphicsDevice();

			video = device.Content.Load<Media.Video>(filename);
		}

		private readonly Drawing drawing;
		private Media.Video video;
		private readonly XnaDevice graphicsDevice;
		private readonly XnaSoundDevice device;

		internal class UnableToContinueWithoutXnaGraphicsDevice : Exception {}

		protected override VideoSurface PlayNativeVideo(float volume)
		{
			positionInSeconds = 0f;
			device.NativePlayer.Volume = volume;
			device.NativePlayer.Play(video);
			return new XnaVideoSurface(this, drawing, renderer, graphicsDevice, device);
		}

		private float positionInSeconds;

		protected override void StopNativeVideo()
		{
			device.NativePlayer.Stop();
		}

		public override bool IsPlaying()
		{
			return device.NativePlayer.State != Media.MediaState.Stopped && IsActiveVideo();
		}

		private bool IsActiveVideo()
		{
			return device.NativePlayer.Video == video;
		}

		protected override void Run()
		{
			if (!IsActiveVideo())
				return;

			positionInSeconds = (float)device.NativePlayer.PlayPosition.TotalSeconds;
		}

		public override void Dispose()
		{
			video = null;
		}

		public override float DurationInSeconds
		{
			get { return (float)video.Duration.TotalSeconds; }
		}

		public override float PositionInSeconds
		{
			get { return positionInSeconds; }
		}
	}
}