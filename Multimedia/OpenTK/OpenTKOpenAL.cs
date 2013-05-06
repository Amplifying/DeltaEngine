using DeltaEngine.Datatypes;
using DeltaEngine.Multimedia.OpenTK.Helpers;
using OpenTK.Audio.OpenAL;

namespace DeltaEngine.Multimedia.OpenTK
{
	public class OpenTKOpenAL
	{
		public int CreateBuffer()
		{
			return AL.GenBuffer();
		}

		public int[] CreateBuffers(int numberOfBuffers)
		{
			return AL.GenBuffers(numberOfBuffers);
		}

		public void DeleteBuffer(int bufferHandle)
		{
			AL.DeleteBuffer(bufferHandle);
		}

		public void DeleteBuffers(int[] bufferHandles)
		{
			AL.DeleteBuffers(bufferHandles);
		}

		public void BufferData(int bufferHandle, AudioFormat format, byte[] data, int length, int 
			sampleRate)
		{
			AL.BufferData(bufferHandle, AudioFormatToALFormat(format), data, length, sampleRate);
		}

		public int CreateChannel()
		{
			return AL.GenSource();
		}

		public void DeleteChannel(int channelHandle)
		{
			AL.DeleteSource(channelHandle);
		}

		public void AttachBufferToChannel(int bufferHandle, int channelHandle)
		{
			AL.Source(channelHandle, ALSourcei.Buffer, bufferHandle);
		}

		public void QueueBufferInChannel(int bufferHandle, int channelHandle)
		{
			AL.SourceQueueBuffer(channelHandle, bufferHandle);
		}

		public int UnqueueBufferFromChannel(int channelHandle)
		{
			return AL.SourceUnqueueBuffer(channelHandle);
		}

		public int GetNumberOfBuffersQueued(int channelHandle)
		{
			int numberOfBuffersQueued;
			AL.GetSource(channelHandle, ALGetSourcei.BuffersQueued, out numberOfBuffersQueued);
			return numberOfBuffersQueued;
		}

		public int GetNumberOfBuffersProcesed(int channelHandle)
		{
			int numberOfBufferProcessed;
			AL.GetSource(channelHandle, ALGetSourcei.BuffersProcessed, out numberOfBufferProcessed);
			return numberOfBufferProcessed;
		}

		public ChannelState GetChannelState(int channelHandle)
		{
			int sourceState;
			AL.GetSource(channelHandle, ALGetSourcei.SourceState, out sourceState);
			return ALSourceStateToChannelState((ALSourceState)sourceState);
		}

		public void SetVolume(int channelHandle, float volume)
		{
			AL.Source(channelHandle, ALSourcef.Gain, volume);
		}

		public void SetPosition(int channelHandle, Vector position)
		{
			AL.Source(channelHandle, ALSource3f.Position, position.X, position.Y, position.Z);
		}

		public void SetPitch(int channelHandle, float pitch)
		{
			AL.Source(channelHandle, ALSourcef.Pitch, pitch);
		}

		public void Play(int channelHandle)
		{
			AL.SourcePlay(channelHandle);
		}

		public void Stop(int channelHandle)
		{
			AL.SourceStop(channelHandle);
		}

		public bool IsPlaying(int channelHandle)
		{
			return AL.GetSourceState(channelHandle) == ALSourceState.Playing;
		}

		private static ALFormat AudioFormatToALFormat(AudioFormat audioFormat)
		{
			switch (audioFormat)
			{
				case AudioFormat.Mono8:
					return ALFormat.Mono8;
				case AudioFormat.Mono16:
					return ALFormat.Mono16;
				case AudioFormat.Stereo8:
					return ALFormat.Stereo8;
				default:
					return ALFormat.Stereo16;
			}
		}

		private static ChannelState ALSourceStateToChannelState(ALSourceState alSourceState)
		{
			switch (alSourceState)
			{
				case ALSourceState.Playing:
					return ChannelState.Playing;
				case ALSourceState.Paused:
					return ChannelState.Paused;
				default:
					return ChannelState.Stopped;
			}
		}
	}
}