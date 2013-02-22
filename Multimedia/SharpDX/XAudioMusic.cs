using System;
using System.IO;
using DeltaEngine.Core;
using Mp3Reader;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace DeltaEngine.Multimedia.SharpDX
{
	/// <summary>
	/// Native XAudio implementation for music playback.
	/// </summary>
	public class XAudioMusic : Music
	{
		public XAudioMusic(string filename, XAudioDevice device)
			: base(filename, device)
		{
			musicStream = new Mp3Stream(File.OpenRead("Content/" + filename + ".mp3"));
			source = new SourceVoice(device.XAudio2,
				new WaveFormat(musicStream.Samplerate, 16, musicStream.Channels), false);
			CreateBuffers();
		}

		private Mp3Stream musicStream;
		private SourceVoice source;
		private StreamBuffer[] buffers;
		private int nextBufferIndex;
		private const int NumberOfBuffers = 2;

		private void CreateBuffers()
		{
			buffers = new StreamBuffer[NumberOfBuffers];
			for (int i = 0; i < NumberOfBuffers; i++)
				buffers[i] = new StreamBuffer();
		}

		protected override void PlayNativeMusic(float volume)
		{
			if (isPlaying)
				return;

			source.Start();
			isPlaying = true;
			playStartTime = DateTime.Now;
		}

		private DateTime playStartTime;
		private bool isPlaying;

		protected override void Run()
		{
			if (isPlaying)
				RunIfPlaying();
		}

		private void RunIfPlaying()
		{
			while (source.State.BuffersQueued < NumberOfBuffers)
				if (!TryStream())
				{
					Stop();
					break;
				}
		}

		public override void Stop()
		{
			if (!isPlaying)
				return;

			isPlaying = false;
			source.Stop();
			source.FlushSourceBuffers();
		}

		public override bool IsPlaying()
		{
			return isPlaying;
		}

		private bool TryStream()
		{
			StreamBuffer currentBuffer = buffers[nextBufferIndex];
			bool wasAbleToStream = currentBuffer.FillFromStream(musicStream);
			if (wasAbleToStream)
			{
				source.SubmitSourceBuffer(currentBuffer.XAudioBuffer, null);
				nextBufferIndex = (nextBufferIndex + 1) % NumberOfBuffers;
			}

			return wasAbleToStream;
		}

		public override void Dispose()
		{
			if (musicStream == null)
				return;

			Stop();
			musicStream = null;

			if (source != null)
				DisposeSource();
		}

		private void DisposeSource()
		{
			for (int i = 0; i < NumberOfBuffers; i++)
				buffers[i].Dispose();

			source.FlushSourceBuffers();
			source.DestroyVoice();
			source.Dispose();
			source = null;
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