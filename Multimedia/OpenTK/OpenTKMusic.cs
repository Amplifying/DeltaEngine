using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Multimedia.OpenTK.Helpers;
using ToyMp3;
using System.Diagnostics;
using DeltaEngine.Logging;

namespace DeltaEngine.Multimedia.OpenTK
{
	public class OpenTKMusic : Music
	{
		public OpenTKMusic(string filename, SoundDevice device, Logger log) : base(filename, device)
		{
			this.openAL = new OpenTKOpenAL();
			channelHandle = openAL.CreateChannel();
			buffers = openAL.CreateBuffers(NumberOfBuffers);
			bufferData = new byte[BufferSize];
			this.log = log;
		}

		private readonly Logger log;
		private OpenTKOpenAL openAL;
		private int channelHandle;
		private int[] buffers;
		private byte[] bufferData;
		private const int NumberOfBuffers = 2;
		private const int BufferSize = 1024 * 8;
		private Mp3Stream musicStream;
		private AudioFormat format;
		private DateTime playStartTime;

		public override float DurationInSeconds
		{
			get
			{
				return musicStream.LengthInSeconds;
			}
		}

		public override float PositionInSeconds
		{
			get
			{
				var seconds = (float)DateTime.Now.Subtract(playStartTime).TotalSeconds;
				return MathExtensions.Round(seconds.Clamp(0f, DurationInSeconds), 2);
			}
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				var stream = new MemoryStream();
				fileData.CopyTo(stream);
				musicStream = new Mp3Stream(stream);
				format = musicStream.Channels == 2 ? AudioFormat.Stereo16 : AudioFormat.Mono16;
			}
			catch (Exception ex)
			{
				ExecuteLogger(ex);
				if (Debugger.IsAttached)
					throw new MusicNotFoundOrAccessible(Name, ex);
			}
		}

		protected override void PlayNativeMusic(float volume)
		{
			for (int index = 0; index < NumberOfBuffers; index++)
				if (!Stream(buffers [index]))
					break;

			openAL.Play(channelHandle);
			openAL.SetVolume(channelHandle, volume);
			playStartTime = DateTime.Now;
		}

		protected override void StopNativeMusic()
		{
			openAL.Stop(channelHandle);
			EmptyBuffers();
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

		protected override void Run()
		{
			if (GetState() == ChannelState.Paused)
				return;

			bool isFinished = UpdateBuffersAndCheckFinished();
			if (isFinished)
			{
				Stop();
				return;
			}
			if (GetState() != ChannelState.Playing)
				openAL.Play(channelHandle);
		}

		private ChannelState GetState()
		{
			return openAL.GetChannelState(channelHandle);
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

		private bool Stream(int buffer)
		{
			int bytesRead = musicStream.Read(bufferData, BufferSize);
			if (bytesRead == 0)
				return false;

			openAL.BufferData(buffer, format, bufferData, bytesRead, musicStream.Samplerate);
			openAL.QueueBufferInChannel(buffer, channelHandle);
			return true;
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			openAL.DeleteBuffers(buffers);
			openAL.DeleteChannel(channelHandle);
			musicStream = null;
		}

		private void ExecuteLogger(Exception ex)
		{
			log.Error(ex);
		}
	}
}