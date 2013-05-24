using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace NewDeltaEngineProject
{
	public class Game : Runner<Window>
	{
		public void Run(Window window)
		{
			FadePercentage += Time.Current.Delta;
			if (FadePercentage >= 1.0f)
				SwitchToNextRandomColor();

			window.BackgroundColor = Color.Lerp(CurrentColor, NextColor, FadePercentage);
		}

		public float FadePercentage
		{
			get;
			private set;
		}

		public Color CurrentColor
		{
			get;
			private set;
		}

		public Color NextColor
		{
			get;
			private set;
		}

		private void SwitchToNextRandomColor()
		{
			CurrentColor = NextColor;
			NextColor = Color.GetRandomColor();
			FadePercentage = 0;
		}
	}
}
