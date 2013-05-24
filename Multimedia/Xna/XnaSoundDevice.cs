using Microsoft.Xna.Framework;

namespace DeltaEngine.Multimedia.Xna
{
	/// <summary>
	/// Native implementation of a SoundDevice using xna that calls the FrameworkDispatcher.
	/// </summary>
	public class XnaSoundDevice : SoundDevice
	{
		public override void Run()
		{
			base.Run();
			FrameworkDispatcher.Update();
		}
	}
}