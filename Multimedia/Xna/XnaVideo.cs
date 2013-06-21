using System;
using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Sprites;
using Microsoft.Xna.Framework.Media;
using Media = Microsoft.Xna.Framework.Media;
using System.Diagnostics;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native Xna implementation for video playback.
	/// </summary>
	public class XnaVideo : Video
	{
		public XnaVideo(string filename, VideoPlayer player, VideoRenderingDependencies rendering,
			SoundDevice device)
			: base(filename, device)
		{
			this.player = player;
			this.rendering = rendering;
			image = new VideoImage(rendering, player);
		}

		private readonly VideoPlayer player;
		private readonly VideoRenderingDependencies rendering;
		private Media.Video video;
		private readonly VideoImage image;

		protected override void PlayNativeVideo(float volume)
		{
			positionInSeconds = 0f;
			player.Volume = volume;
			player.Play(video);
			surface = new Sprite(image, rendering.Screen.Viewport, Color.White);
		}

		private Sprite surface;
		private float positionInSeconds;

		protected override void StopNativeVideo()
		{
			if (surface != null)
				surface.IsActive=false;

			surface = null;
			player.Stop();
		}

		public override bool IsPlaying()
		{
			return player.State != MediaState.Stopped && IsActiveVideo();
		}

		private bool IsActiveVideo()
		{
			return player.Video == video;
		}

		protected override void Run()
		{
			if (!IsActiveVideo())
				return;

			image.UpdateTexture();
			positionInSeconds = (float)player.PlayPosition.TotalSeconds;
		}

		protected override bool CanLoadDataFromStream
		{
			get { return false; }
		}

		protected override void LoadData(Stream fileData)
		{
			throw new XnaOnlyAllowsLoadingThroughContentNames();
		}

		public class XnaOnlyAllowsLoadingThroughContentNames : Exception {}

		protected override void LoadFromContentName(string contentName)
		{
			try
			{
				video = rendering.NativeContent.Load<Media.Video>(contentName);
			}
			catch (Exception ex)
			{
				//logger.Error(ex);
				if (Debugger.IsAttached)
					throw new XnaVideoContentNotFound(contentName, ex);
			}
		}

		private class XnaVideoContentNotFound : Exception
		{
			public XnaVideoContentNotFound(string contentName, Exception exception)
				: base(contentName, exception) {}
		}

		protected override void DisposeData()
		{
			base.DisposeData();
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