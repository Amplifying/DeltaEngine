using DeltaEngine.Multimedia;

namespace DeltaEngine.Platforms.Mocks
{
	public class MockSoundDevice : SoundDevice
	{
		public override bool IsInitialized
		{
			get { return true; }
		}
	}
}