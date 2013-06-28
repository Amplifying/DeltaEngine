using SharpDX.XAudio2;

namespace DeltaEngine.Multimedia.SharpDX
{
	/// <summary>
	/// Native implementation of an audio context.
	/// </summary>
	public class XAudioDevice : SoundDevice
	{
		public XAudioDevice()
		{
			XAudio2 = new XAudio2();
			MasteringVoice = new MasteringVoice(XAudio2);
		}

		public XAudio2 XAudio2 { get; private set; }
		public MasteringVoice MasteringVoice { get; private set; }

		public override bool IsInitialized
		{
			get { return XAudio2 == null; }
		}

		public override void Run()
		{
			base.Run();
			XAudio2.CommitChanges(XAudio2.CommitAll);
		}

		public override void Dispose()
		{
			base.Dispose();
			DisposeXAudio();
			DisposeMasteringVoice();
		}

		private void DisposeXAudio()
		{
			if (XAudio2 != null)
				XAudio2.Dispose();
			XAudio2 = null;
		}

		private void DisposeMasteringVoice()
		{
			if (MasteringVoice != null)
				MasteringVoice.Dispose();
			MasteringVoice = null;
		}
	}
}