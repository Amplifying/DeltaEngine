using Microsoft.Xna.Framework;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native implementation of a SoundDevice using xna that calls the FrameworkDispatcher.
	/// </summary>
	public sealed class XnaSoundDevice : SoundDevice
	{
		public XnaSoundDevice()
		{
			isInitialized = true;
		}

		public override bool IsInitialized
		{
			get { return isInitialized; }
		}

		private bool isInitialized;

		public override void Run()
		{
			base.Run();
			FrameworkDispatcher.Update();
		}

		public override void Dispose()
		{
			base.Dispose();
			isInitialized = false;
		}
	}
}