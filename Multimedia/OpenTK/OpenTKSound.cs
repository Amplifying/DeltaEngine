using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Logging;
using DeltaEngine.Multimedia.OpenTK.Helpers;
using DeltaEngine.Platforms;

namespace DeltaEngine.Multimedia.OpenTK
{
	/// <summary>
	/// Base for OpenTK implementations for loading and playing sound effects.
	/// </summary>
	public class OpenTKSound : Sound
	{
		public OpenTKSound(string filename, OpenTKSoundDevice openAL, Settings settings)
			: base(filename, settings)
		{
			this.openAL = openAL;
		}

		protected OpenTKSoundDevice openAL;

		protected override void LoadData(Stream fileData)
		{
			try
			{
				LoadSound(fileData);
			}
			catch (Exception ex)
			{
				Logger.Current.Error(ex);
				if (Debugger.IsAttached)
					throw new SoundNotFoundOrAccessible(Name, ex);
			}
		}

		protected void LoadSound(Stream fileData)
		{
			var streamReader = new BinaryReader(fileData);
			soundData = new WaveSoundData(streamReader);
			length = GetLengthInSeconds();
			bufferHandle = CreateNativeBuffer();
		}

		protected WaveSoundData soundData;
		protected float length;
		protected int bufferHandle;

		protected float GetLengthInSeconds()
		{
			float blockAlign = soundData.Channels * 2f;
			return (soundData.BufferData.Length / blockAlign) / soundData.SampleRate;
		}

		protected int CreateNativeBuffer()
		{
			int newHandle = openAL.CreateBuffer();
			openAL.BufferData(newHandle, soundData.Format, soundData.BufferData, 
				soundData.BufferData.Length, soundData.SampleRate);

			return newHandle;
		}

		protected override void DisposeData()
		{
			base.DisposeData();
			if(bufferHandle != InvalidHandle)
				openAL.DeleteBuffer(bufferHandle);

			bufferHandle = InvalidHandle;
		}

		protected const int InvalidHandle = -1;

		public override float LengthInSeconds
		{
			get { return length; }
		}

		public override void PlayInstance(SoundInstance instanceToPlay)
		{
			var channelHandle = (int)instanceToPlay.Handle;
			if(channelHandle == InvalidHandle)
				return;

			openAL.SetVolume(channelHandle, instanceToPlay.Volume);
			openAL.SetPosition(channelHandle, new Vector(instanceToPlay.Panning, 0.0f, 0.0f));
			openAL.SetPitch(channelHandle, instanceToPlay.Pitch);
			openAL.Play(channelHandle);
		}

		public override void StopInstance(SoundInstance instanceToStop)
		{
			var channelHandle = (int)instanceToStop.Handle;
			if(channelHandle == InvalidHandle)
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
			if(channelHandle != InvalidHandle)
				openAL.DeleteChannel(channelHandle);

			instanceToRemove.Handle = InvalidHandle;
		}

		public override bool IsPlaying(SoundInstance instanceToCheck)
		{
			var channelHandle = (int)instanceToCheck.Handle;
			return channelHandle != InvalidHandle && openAL.IsPlaying(channelHandle);
		}
	}
}