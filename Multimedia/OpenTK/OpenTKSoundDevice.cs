using System;
using OpenTK;
using OpenTK.Audio.OpenAL;

namespace DeltaEngine.Multimedia.OpenTK
{
	public sealed class OpenTKSoundDevice : SoundDevice
	{
		public OpenTKSoundDevice()
		{
			InitDeviceAndContext();
		}

		private IntPtr deviceHandle;
		private ContextHandle context;

		public override void Dispose()
		{
			ReleaseDeviceAndContext();
		}

		private void InitDeviceAndContext()
		{
			deviceHandle = Alc.OpenDevice("");
			context = Alc.CreateContext(deviceHandle, new int[0]);
			Alc.MakeContextCurrent(context);
		}

		private void ReleaseDeviceAndContext()
		{
			if (deviceHandle == IntPtr.Zero)
				return;

			Alc.DestroyContext(context);
			Alc.CloseDevice(deviceHandle);
			deviceHandle = IntPtr.Zero;
		}
	}
}