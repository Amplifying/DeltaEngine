using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;

namespace DeltaEngine.Multimedia.SharpDX
{
	/// <summary>
	/// Native SharpDX implementation of Sound.
	/// </summary>
	public class XAudioSound : Sound
	{
		public XAudioSound(string filename, XAudioDevice device)
			: base(filename, device)
		{
			xAudio = device.XAudio2;
			using (var stream = LoadStream("Content/" + filename + ".wav"))
			{
				format = stream.Format;
				length = CalculateLengthInSeconds(format, (int)stream.Length);
				buffer = CreateAudioBuffer(stream.ToDataStream());
				decodedInfo = stream.DecodedPacketsInfo;
			}
		}

		private readonly XAudio2 xAudio;
		private readonly uint[] decodedInfo;
		private readonly WaveFormat format;
		private AudioBuffer buffer;

		private static SoundStream LoadStream(string filename)
		{
			return new SoundStream(File.OpenRead(filename));
		}

		private static AudioBuffer CreateAudioBuffer(DataStream dataStream)
		{
			return new AudioBuffer
			{
				Stream = dataStream,
				AudioBytes = (int)dataStream.Length,
				Flags = BufferFlags.EndOfStream
			};
		}

		private static float CalculateLengthInSeconds(WaveFormat format, int dataLength)
		{
			return (float)dataLength / format.BlockAlign / format.SampleRate;
		}

		private readonly float length;

		public override float LengthInSeconds
		{
			get { return length; }
		}

		public override void Dispose()
		{
			base.Dispose();
			if (buffer != null)
				buffer.Stream.Dispose();
			buffer = null;
		}

		public override void PlayInstance(SoundInstance instanceToPlay)
		{
			var soundInstance = instanceToPlay.Handle as SourceVoice;
			if (soundInstance == null)
				return;

			soundInstance.SubmitSourceBuffer(buffer, decodedInfo);
			soundInstance.SetVolume(instanceToPlay.Volume);
			float left = 0.5f - instanceToPlay.Panning / 2;
			float right = 0.5f + instanceToPlay.Panning / 2;
			soundInstance.SetOutputMatrix(1, 2, new[] { left, right });
			soundInstance.SetFrequencyRatio(instanceToPlay.Pitch);
			soundInstance.Start();
			instancesPlaying.Add(instanceToPlay);
		}

		private readonly List<SoundInstance> instancesPlaying = new List<SoundInstance>();

		public override void StopInstance(SoundInstance instanceToStop)
		{
			var soundInstance = instanceToStop.Handle as SourceVoice;
			if (soundInstance != null)
				soundInstance.Stop();

			if (instancesPlaying.Contains(instanceToStop))
				instancesPlaying.Remove(instanceToStop);
		}

		protected override void CreateChannel(SoundInstance instanceToFill)
		{
			if (buffer == null)
				return;

			var source = new SourceVoice(xAudio, format, true);
			source.StreamEnd += () => instancesPlaying.Remove(instanceToFill);
			instanceToFill.Handle = source;
		}

		protected override void RemoveChannel(SoundInstance instanceToRemove)
		{
			var soundInstance = instanceToRemove.Handle as SourceVoice;
			if (soundInstance != null)
			{
				soundInstance.Stop();
				soundInstance.Dispose();
			}

			instanceToRemove.Handle = null;
		}

		public override bool IsPlaying(SoundInstance instance)
		{
			return instancesPlaying.Contains(instance);
		}
	}
}