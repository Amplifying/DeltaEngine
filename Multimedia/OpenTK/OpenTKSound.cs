using OpenTK.Audio.OpenAL;

namespace DeltaEngine.Multimedia.OpenTK
{
	/// <summary>
	/// Native OpenTK implementation of loading and playing sound effects.
	/// </summary>
	public class OpenTKSound : Sound
	{
		public OpenTKSound(string filename, SoundDevice device)
			: base(filename, device)
		{
			soundData = new WaveSoundData("Content/" + filename + ".wav");
			length = GetLengthInSeconds();
			bufferHandle = CreateNativeBuffer();
		}

		private readonly WaveSoundData soundData;
		private readonly float length;
		private int bufferHandle;

		private float GetLengthInSeconds()
		{
			float blockAlign = soundData.Channels * 2f;
			return (soundData.BufferData.Length / blockAlign) / soundData.SampleRate;
		}

		private int CreateNativeBuffer()
		{
			int newHandle = AL.GenBuffer();
			AL.BufferData(newHandle, soundData.Format, soundData.BufferData, soundData.BufferData.Length,
				soundData.SampleRate);
			return newHandle;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (bufferHandle != InvalidHandle)
				AL.DeleteBuffer(bufferHandle);
			bufferHandle = InvalidHandle;
		}

		private const int InvalidHandle = -1;

		public override float LengthInSeconds
		{
			get { return length; }
		}

		public override void PlayInstance(SoundInstance instanceToPlay)
		{
			var handle = (int)instanceToPlay.Handle;
			if (handle == InvalidHandle)
				return;

			AL.Source(handle, ALSourcef.Gain, instanceToPlay.Volume);
			AL.Source(handle, ALSource3f.Position, instanceToPlay.Panning, 0, 0);
			AL.Source(handle, ALSourcef.Pitch, instanceToPlay.Pitch);
			AL.SourcePlay(handle);
		}

		public override void StopInstance(SoundInstance instanceToStop)
		{
			var handle = (int)instanceToStop.Handle;
			if (handle == InvalidHandle)
				return;

			AL.SourceStop(handle);
		}

		protected override void CreateChannel(SoundInstance instanceToAdd)
		{
			var handle = AL.GenSource();
			AL.Source(handle, ALSourcei.Buffer, bufferHandle);
			instanceToAdd.Handle = handle;
		}

		protected override void RemoveChannel(SoundInstance instanceToRemove)
		{
			var handle = (int)instanceToRemove.Handle;
			if (handle != InvalidHandle)
				AL.DeleteSource(handle);
			instanceToRemove.Handle = InvalidHandle;
		}

		public override bool IsPlaying(SoundInstance instanceToCheck)
		{
			var handle = (int)instanceToCheck.Handle;
			return handle != InvalidHandle && AL.GetSourceState(handle) == ALSourceState.Playing;
		}
	}
}