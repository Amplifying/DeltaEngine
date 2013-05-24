using DeltaEngine.Datatypes;
using DeltaEngine.Multimedia.OpenTK.Helpers;
using System;
using System.IO;
using System.Diagnostics;
using DeltaEngine.Logging;

namespace DeltaEngine.Multimedia.OpenTK
{
	public class OpenTKSound : Sound
	{
		public OpenTKSound(string contentName, SoundDevice device, Logger log) : base(contentName, 
			device)
		{
			this.openAL = new OpenTKOpenAL();
			this.log = log;
		}

		private readonly Logger log;
		private OpenTKOpenAL openAL;
		private WaveSoundData soundData;
		private float length;
		private int bufferHandle;
		private const int InvalidHandle = -1;

		public override float LengthInSeconds
		{
			get
			{
				return length;
			}
		}

		private float GetLengthInSeconds()
		{
			float blockAlign = soundData.Channels * 2f;
			return (soundData.BufferData.Length / blockAlign) / soundData.SampleRate;
		}

		private int CreateNativeBuffer()
		{
			int newHandle = openAL.CreateBuffer();
			openAL.BufferData(newHandle, soundData.Format, soundData.BufferData, 
				soundData.BufferData.Length, soundData.SampleRate);
			return newHandle;
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			if (bufferHandle != InvalidHandle)
				openAL.DeleteBuffer(bufferHandle);

			bufferHandle = InvalidHandle;
		}

		public override void PlayInstance(SoundInstance instanceToPlay)
		{
			var channelHandle = (int)instanceToPlay.Handle;
			if (channelHandle == InvalidHandle)
				return;

			openAL.SetVolume(channelHandle, instanceToPlay.Volume);
			openAL.SetPosition(channelHandle, new Vector(instanceToPlay.Panning, 0.0f, 0.0f));
			openAL.SetPitch(channelHandle, instanceToPlay.Pitch);
			openAL.Play(channelHandle);
		}

		public override void StopInstance(SoundInstance instanceToStop)
		{
			var channelHandle = (int)instanceToStop.Handle;
			if (channelHandle == InvalidHandle)
				return;

			openAL.Stop(channelHandle);
		}

		protected override void CreateChannel(SoundInstance instanceToAdd)
		{
			var channelHandle = openAL.CreateChannel();
			openAL.AttachBufferToChannel(bufferHandle, channelHandle);
			instanceToAdd.Handle = channelHandle;
		}

		protected override void RemoveChannel(SoundInstance instanceToRemove)
		{
			var channelHandle = (int)instanceToRemove.Handle;
			if (channelHandle != InvalidHandle)
				openAL.DeleteChannel(channelHandle);

			instanceToRemove.Handle = InvalidHandle;
		}

		public override bool IsPlaying(SoundInstance instanceToCheck)
		{
			var channelHandle = (int)instanceToCheck.Handle;
			return channelHandle != InvalidHandle && openAL.IsPlaying(channelHandle);
		}

		protected override void LoadData(Stream fileData)
		{
			try
			{
				byte[] loadedData = ReadStream(fileData);
				Stream loadedDataStream = new MemoryStream(loadedData);
				var loadedStreamReader = new BinaryReader(loadedDataStream);
				soundData = new WaveSoundData(loadedStreamReader);
				length = GetLengthInSeconds();
				bufferHandle = CreateNativeBuffer();
			}
			catch (Exception ex)
			{
				log.Error(ex);
				if (Debugger.IsAttached)
					throw new SoundNotFoundOrAccessible(Name, ex);
			}
		}

		[Obsolete("TODO: This code is kind of stupid, why not just call File.Open")]
		protected byte[] ReadStream(Stream fileData)
		{
			const int InitialBufferLength = 32768;
			var readBuffer = new byte[InitialBufferLength];
			int readOffset = 0;
			int currentChunk;
			while ((currentChunk = fileData.Read(readBuffer, readOffset, readBuffer.Length - 
				readOffset)) > 0)
			{
				readOffset += currentChunk;
				if (readOffset == readBuffer.Length)
				{
					int nextByte = fileData.ReadByte();
					if (nextByte == -1)
						return readBuffer;

					var newBuffer = new byte[readBuffer.Length * 2];
					Array.Copy(readBuffer, newBuffer, readBuffer.Length);
					newBuffer [readOffset] = (byte)nextByte;
					readBuffer = newBuffer;
					readOffset++;
				}
			}
			var returnBuffer = new byte[readOffset];
			Array.Copy(readBuffer, returnBuffer, readOffset);
			return returnBuffer;
		}
	}
}