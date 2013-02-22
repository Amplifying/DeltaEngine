namespace DeltaEngine.Input
{
	/// <summary>
	/// Extensions to make it easier to work with Input states used for keys, buttons and gestures.
	/// </summary>
	public static class StateExtensions
	{
		public static bool PressingOrPressed(this State state)
		{
			return state == State.Pressing || state == State.Pressed;
		}

		public static State UpdateOnNativePressing(this State currentState, bool isNativePressing)
		{
			return isNativePressing
				? currentState.PressingOrPressed() ? State.Pressed : State.Pressing
				: !currentState.PressingOrPressed() ? State.Released : State.Releasing;
		}
	}
}