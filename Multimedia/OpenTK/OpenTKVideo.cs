using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Multimedia.AviVideo;
using DeltaEngine.Multimedia.OpenTK.Helpers;
using DeltaEngine.Rendering.ScreenSpaces;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Logging;

namespace DeltaEngine.Multimedia.OpenTK
{
	public class OpenTKVideo : Video
	{
		public OpenTKVideo(string filename, SoundDevice device, Drawing drawing, ScreenSpace screen, 
			Logger log) : base(filename, device)
		{
			this.screen = screen;
			this.log = log;
			openAL = new OpenTKOpenAL();
			channelHandle = openAL.CreateChannel();
			buffers = openAL.CreateBuffers(NumberOfBuffers);
			image = new VideoImage();
		}

		private readonly Logger log;
		private readonly VideoImage image;
		private ScreenSpace screen;
		private OpenTKOpenAL openAL;
		private int channelHandle;
		private int[] buffers;
		private const int NumberOfBuffers = 4;
		private VideoStream video;
		private AudioStream audio;
		private AudioFormat format;
		private int lastFrameIndex = -1;
		private Sprite surface;
		private float elapsedSeconds;

		public override float DurationInSeconds
		{
			get
			{
				return (float)(video.CountFrames / video.FrameRate);
			}
		}

		public override float PositionInSeconds
		{
			get
			{
				return MathExtensions.Round(elapsedSeconds.Clamp(0f, DurationInSeconds), 2);
			}
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				var aviManager = new AviFile("Content/DefaultVideo.avi");
				video = aviManager.GetVideoStream();
				audio = aviManager.GetAudioStream();
				format = audio.Channels == 2 ? AudioFormat.Stereo16 : AudioFormat.Mono16;
			}
			catch (Exception ex)
			{
				ExecuteLogger(ex);
				if (Debugger.IsAttached)
					throw new VideoNotFoundOrAccessible(Name, ex);
			}
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			openAL.DeleteBuffers(buffers);
			openAL.DeleteChannel(channelHandle);
			video = null;
			audio = null;
		}

		private bool Stream(int buffer)
		{
			try
			{
				byte[] bufferData = audio.GetStreamData();
				openAL.BufferData(buffer, format, bufferData, bufferData.Length, audio.SamplesPerSecond);
				openAL.QueueBufferInChannel(buffer, channelHandle);
			}
			catch
			{
				return false;
			}
			return true;
		}

		protected override void StopNativeVideo()
		{
			if (surface != null)
				surface.IsActive = false;

			surface = null;
			openAL.Stop(channelHandle);
			EmptyBuffers();
			video.GetFrameClose();
		}

		private void EmptyBuffers()
		{
			int queued = openAL.GetNumberOfBuffersQueued(channelHandle);
			while (queued-- > 0)
				openAL.UnqueueBufferFromChannel(channelHandle);
		}

		public override bool IsPlaying()
		{
			return GetState() != ChannelState.Stopped;
		}

		private ChannelState GetState()
		{
			return openAL.GetChannelState(channelHandle);
		}

		protected override void Run()
		{
			if (GetState() == ChannelState.Paused)
				return;

			elapsedSeconds += Time.Current.Delta;
			bool isFinished = UpdateBuffersAndCheckFinished();
			if (isFinished)
			{
				Stop();
				return;
			}
			UpdateVideoTexture();
			if (GetState() != ChannelState.Playing)
				openAL.Play(channelHandle);
		}

		private bool UpdateBuffersAndCheckFinished()
		{
			int processed = openAL.GetNumberOfBuffersProcesed(channelHandle);
			while (processed-- > 0)
			{
				int buffer = openAL.UnqueueBufferFromChannel(channelHandle);
				if (!Stream(buffer))
					return true;
			}
			return false;
		}

		protected override void PlayNativeVideo(float volume)
		{
			for (int index = 0; index < NumberOfBuffers; index++)
				if (!Stream(buffers [index]))
					break;

			video.GetFrameOpen();
			openAL.Play(channelHandle);
			openAL.SetVolume(channelHandle, volume);
			elapsedSeconds = 0f;
			surface = new Sprite(image, screen.Viewport, Color.White);
		}

		private void ExecuteLogger(Exception ex)
		{
			log.Error(ex);
		}

		private void UpdateVideoTexture()
		{
			var frameIndex = (int)(PositionInSeconds * video.FrameRate);
			if (lastFrameIndex >= frameIndex)
				return;

			lastFrameIndex = frameIndex;
			image.UpdateTexture(video, frameIndex);
		}
	}
}