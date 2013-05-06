namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Component holding the render layer for a rendered entity. If not used defaults to 0.
	/// </summary>
	public struct RenderLayer
	{
		public RenderLayer(int value)
			: this()
		{
			Value = value;
		}

		public int Value { get; set; }
	}
}