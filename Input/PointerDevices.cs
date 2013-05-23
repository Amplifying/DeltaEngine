namespace DeltaEngine.Input
{
	/// <summary>
	/// Mouse and touch devices can both be used as pointers, only used in InputCommands.
	/// </summary>
	public sealed class PointerDevices
	{
		public PointerDevices(Mouse mouse, Touch touch)
		{
			this.mouse = mouse;
			this.touch = touch;
		}

		internal readonly Mouse mouse;
		internal readonly Touch touch;
	}
}
