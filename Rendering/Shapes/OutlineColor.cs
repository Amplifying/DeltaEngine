using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Shapes
{
	/// <summary>
	/// Stores the color of the outline to a shape
	/// </summary>
	public class OutlineColor
	{
		public OutlineColor(Color color)
		{
			Value = color;
		}

		public Color Value { get; set; }
	}
}