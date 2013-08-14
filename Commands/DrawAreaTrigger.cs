using DeltaEngine.Datatypes;

namespace DeltaEngine.Commands
{
	/// <summary>
	/// Allows a trigger to be invoked along with any attached Command or Entity.
	/// Returns a rectangle.
	/// </summary>
	public abstract class DrawAreaTrigger : Trigger
	{
		protected DrawAreaTrigger(Rectangle drawArea)
		{
			DrawArea = drawArea;
		}

		public Rectangle DrawArea { get; set; }
	}
}