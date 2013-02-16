using System;

namespace DeltaEngine.Input.Devices
{
	/// <summary>
	/// Helper class for updating a specific InputState lifecycle.
	/// </summary>
	[Obsolete("Remove this")]
	public class InputStateHelper
	{
		public State UpdateAndGetStateNowPressed(State previousState)
		{
			return IsPreviousReleasedOrNotPressed(previousState) ? State.JustPressed : State.IsPressed;
		}

		public State UpdateAndGetStateNowReleased(State previousState)
		{
			return IsPreviousReleasedOrNotPressed(previousState) ? State.NotPressed : State.Released;
		}

		private bool IsPreviousReleasedOrNotPressed(State previousState)
		{
			return previousState == State.NotPressed || previousState == State.Released;
		}
	}
}