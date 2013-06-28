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
	/*TODO: would be nice to do
	 * var ellipse = new Ellipse(Point.Half, Size.One, Color.Yellow);
	 * ellipse.Add(new Outline(Color.Red));
	 * 
	 * with:
	 * public class Outline : RenderableComponent
	 * {
	 *   public Outline(Color color)
	 *   {
	 *     Color = color;
	 *   }
	 *   
	 *   public void Render(Drawing draw, ScreenSpace screen)
	 *   {
	 *     draw.DrawVertices(VerticesMode.Lines, GetLinePoints());
	 *   }
	 * }
	 */
}