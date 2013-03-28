using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;

namespace DeltaEngine.Rendering.Tests
{
	/// <summary>
	/// Just draws just a colored box, called via ColoredRectangleTests and by the tool
	/// ContinuousUpdater to test dynamic updating while the app is running (allowing code changes).
	/// </summary>
	public class BlinkingBox : Rect
	{
		public BlinkingBox()
			: base(new Rectangle(Point.Zero, Size.Half), Color.Red) {}

		protected override void Render(Renderer renderer, Time time)
		{
			Color = Color.GetRandomBrightColor();
			base.Render(renderer, time);
		}
	}
}