namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Component holding the rotation for an entity
	/// </summary>
	public class Rotation
	{
		public Rotation()
			: this(0.0f) {}

		public Rotation(float value)
		{
			Value = value;
		}

		public float Value { get; set; }
	}
}