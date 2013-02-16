using DeltaEngine.Input.Triggers;

namespace DeltaEngine.Input.Tests
{
	public class MockCommand : Command
	{
		public void Simulate(Key key)
		{
			foreach (KeyTrigger trigger in attachedTriggers)
				if (trigger != null && trigger.Key == key)
				{
					Invoke(trigger);
					return;
				}
		}

		public void Simulate(MouseButton mouseButton)
		{
			foreach (MouseButtonTrigger trigger in attachedTriggers)
				if (trigger != null && trigger.Button == mouseButton)
				{
					Invoke(trigger);
					return;
				}
		}

		public void SimulateMovement()
		{
			foreach (MouseMovementTrigger trigger in attachedTriggers)
				if (trigger != null)
				{
					Invoke(trigger);
					return;
				}
		}
	}
}