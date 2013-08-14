using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch flick to be detected.
	/// </summary>
	public class TouchFlickTrigger : Trigger
	{
		public TouchFlickTrigger()
		{
			Start<Touch>();
		}
	}
}