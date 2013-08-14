using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch drag to be detected.
	/// </summary>
	public class TouchDragTrigger : Trigger
	{
		public TouchDragTrigger()
		{
			Start<Touch>();
		}
	}
}