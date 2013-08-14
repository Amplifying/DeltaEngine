using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Multimedia.OpenTK.Helpers;
using DeltaEngine.Multimedia.VideoStreams;
using DeltaEngine.Rendering.Sprites;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Multimedia.OpenTK
{
	public class OpenTKVideo : Video
	{
		public OpenTKVideo(string filename, OpenTKSoundDevice soundDevice) : base(filename, soundDevice)
		{
			openAL = soundDevice;
			channelHandle = openAL.CreateChannel();
			buffers = openAL.CreateBuffers(NumberOfBuffers);
		}

		private Image image;
		private readonly OpenTKSoundDevice openAL;
		private int channelHandle;
		private int[] buffers;
		private const int NumberOfBuffers = 4;
		private BaseVideoStream video;
		private AudioFormat format;
		private Sprite surface;
		private float elapsedSeconds;

		public override float DurationInSeconds
		{
			get
			{
				return video.LengthInSeconds;
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
				string filepath = "Content/" + Name + ".wmv";
				video = new WmvVideoStream(filepath);
				format = video.Channels == 2 ? AudioFormat.Stereo16 : AudioFormat.Mono16;
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				if (Debugger.IsAttached)
					throw new VideoNotFoundOrAccessible(Name, ex);
			}
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			openAL.DeleteBuffers(buffers);
			openAL.DeleteChannel(channelHandle);
			video.Dispose();
			video = null;
		}

		private bool Stream(int buffer)
		{
			try
			{
				byte[] bufferData = new byte[4096];
				video.ReadMusicBytes(bufferData, bufferData.Length);
				openAL.BufferData(buffer, format, bufferData, bufferData.Length, video.Samplerate);
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

			elapsedSeconds = 0;
			surface = null;
			openAL.Stop(channelHandle);
			EmptyBuffers();
			video.Stop();
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

		public override void Update()
		{
			if (GetState() == ChannelState.Paused)
				return;

			elapsedSeconds += Time.Delta;
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
			video.Rewind();
			for (int index = 0; index < NumberOfBuffers; index++)
				if (!Stream(buffers [index]))
					break;

			video.Play();
			openAL.Play(channelHandle);
			openAL.SetVolume(channelHandle, volume);
			elapsedSeconds = 0f;
			if (image == null)
				image = ContentLoader.Create<Image>(new ImageCreationData(new Size(video.Width, 
					video.Height)));

			surface = new Sprite(new Material(ContentLoader.Load<Shader>(Shader.Position2DUv), image), 
				ScreenSpace.Current.Viewport);
		}

		private void UpdateVideoTexture()
		{
			byte[] bytes = video.ReadImage(Time.Delta);
			if (bytes != null)
				image.Fill(bytes);
			else
				Stop();
		}
	}
}