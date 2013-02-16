using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace $safeprojectname$
{
	/// <summary>
	/// Just creates a window and slowly changes the background color.
	/// </summary>
	public class Game : Runner<Time, Window>
	{
		public void Run(Time time, Window window)
		{
			fadePercentage += time.CurrentDelta;
			if (fadePercentage >= 1.0f)
				SwitchToNextRandomColor();

			window.BackgroundColor = Color.Lerp(currentColor, nextColor, fadePercentage);
		}

		private float fadePercentage;
		private Color currentColor;
		private Color nextColor;

		private void SwitchToNextRandomColor()
		{
			currentColor = nextColor;
			nextColor = Color.GetRandomColor();
			fadePercentage = 0;
		}
	}
}