using System;
using System.IO;
using DeltaEngine.Core;
using OpenTK.Audio.OpenAL;
using ToyMp3;

namespace DeltaEngine.Multimedia.OpenTK
{
	/// <summary>
	/// Native OpenAL implementation for music playback.
	/// </summary>
	public class OpenTKMusic : Music
	{
		public OpenTKMusic(string filename, SoundDevice device)
			: base(filename, device)
		{
			musicStream = new Mp3Stream(File.OpenRead("Content/" + filename + ".mp3"));
			format = musicStream.Channels == 2 ? ALFormat.Stereo16 : ALFormat.Mono16;
			source = AL.GenSource();
			buffers = AL.GenBuffers(NumberOfBuffers);
			bufferData = new byte[BufferSize];
		}

		private Mp3Stream musicStream;
		private readonly ALFormat format;
		private readonly int source;
		private const int NumberOfBuffers = 2;
		private readonly int[] buffers;
		private readonly byte[] bufferData;
		private const int BufferSize = 1024 * 8;

		protected override void PlayNativeMusic(float volume)
		{
			for (int index = 0; index < NumberOfBuffers; index++)
				if (!Stream(buffers[index]))
					break;

			AL.SourcePlay(source);
			AL.Source(source, ALSourcef.Gain, volume);
			playStartTime = DateTime.Now;
		}

		private DateTime playStartTime;

		public override void Stop()
		{
			AL.SourceStop(source);
			EmptyBuffers();
		}

		private void EmptyBuffers()
		{
			int queued;
			AL.GetSource(source, ALGetSourcei.BuffersQueued, out queued);
			while (queued-- > 0)
				AL.SourceUnqueueBuffer(source);
		}

		public override bool IsPlaying
		{
			get { return GetState() != ALSourceState.Stopped; }
		}

		protected override void Run()
		{
			if (GetState() == ALSourceState.Paused)
				return;

			bool isFinished = UpdateBuffersAndCheckFinished();
			if (isFinished)
			{
				Stop();
				return;
			}

			if (GetState() != ALSourceState.Playing)
				AL.SourcePlay(source);
		}

		private ALSourceState GetState()
		{
			int sourceState;
			AL.GetSource(source, ALGetSourcei.SourceState, out sourceState);
			return (ALSourceState)sourceState;
		}

		private bool UpdateBuffersAndCheckFinished()
		{
			int processed;
			AL.GetSource(source, ALGetSourcei.BuffersProcessed, out processed);
			while (processed > 0)
			{
				int buffer = AL.SourceUnqueueBuffer(source);
				int oldBufferSize;
				AL.GetBuffer(buffer, ALGetBufferi.Size, out oldBufferSize);
				if (!Stream(buffer))
					return true;

				processed--;
			}

			return false;
		}

		private bool Stream(int buffer)
		{
			int bytesRead = musicStream.Read(bufferData, BufferSize);
			if (bytesRead == 0)
				return false;

			AL.BufferData(buffer, format, bufferData, bytesRead, musicStream.Samplerate);
			AL.SourceQueueBuffer(source, buffer);

			return true;
		}

		public override void Dispose()
		{
			AL.DeleteBuffers(buffers);
			AL.DeleteSource(source);
			musicStream = null;
		}

		public override float DurationInSeconds
		{
			get { return musicStream.LengthInSeconds; }
		}

		public override float PositionInSeconds
		{
			get
			{
				float seconds = (float)DateTime.Now.Subtract(playStartTime).TotalSeconds;
				return MathExtensions.Round(seconds.Clamp(0f, DurationInSeconds), 2);
			}
		}
	}
}