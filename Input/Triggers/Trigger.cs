namespace DeltaEngine.Input.Triggers
{
	/// <summary>
	/// Base class for all input command triggers.
	/// </summary>
	public abstract class Trigger
	{
		public abstract bool ConditionMatched(InputCommands inputCommands);
	}
}