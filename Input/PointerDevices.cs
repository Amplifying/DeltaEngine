namespace DeltaEngine.Input
{
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
