using DeltaEngine.Core;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a method to check if an action should be triggered.
	/// </summary>
	public abstract class Trigger
	{
		public abstract bool ConditionMatched(InputCommands inputCommands, Time time);
	}
}