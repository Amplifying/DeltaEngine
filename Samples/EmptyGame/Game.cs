using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace EmptyGame
{
	/// <summary>
	/// Just creates a window and slowly changes the background color.
	/// </summary>
	public class Game : Runner<Time, Window>
	{
		public void Run(Time time, Window window)
		{
			FadePercentage += time.CurrentDelta;
			if (FadePercentage >= 1.0f)
				SwitchToNextRandomColor();

			window.BackgroundColor = Color.Lerp(CurrentColor, NextColor, FadePercentage);
		}

		public float FadePercentage { get; private set; }
		public Color CurrentColor { get; private set; }
		public Color NextColor { get; private set; }

		private void SwitchToNextRandomColor()
		{
			CurrentColor = NextColor;
			NextColor = Color.GetRandomColor();
			FadePercentage = 0;
		}
	}
}