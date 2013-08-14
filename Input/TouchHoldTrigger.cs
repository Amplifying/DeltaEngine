using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch hold to be detected.
	/// </summary>
	public class TouchHoldTrigger : Trigger
	{
		public TouchHoldTrigger()
		{
			Start<Touch>();
		}
	}
}