using System;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace DeltaEngine.Multimedia.OpenTK
{
	/// <summary>
	/// Native implementation of an audio context.
	/// </summary>
	public class OpenTKSoundDevice : SoundDevice
	{
		public OpenTKSoundDevice()
		{
			deviceHandle = Alc.OpenDevice("");
			context = Alc.CreateContext(deviceHandle, new int[0]);
			Alc.MakeContextCurrent(context);
		}

		private readonly ContextHandle context;
		private IntPtr deviceHandle;

		public override void Run()
		{
			//TODO: update anything?
		}

		public override void Dispose()
		{
			if (deviceHandle == IntPtr.Zero)
				return;

			Alc.DestroyContext(context);
			Alc.CloseDevice(deviceHandle);
			deviceHandle = IntPtr.Zero;
		}
	}
}
