namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides the current app run time and delta time for the frame. All times are in seconds.
	/// </summary>
	public class Time : Runner
	{
		public Time(ElapsedTime elapsed)
		{
			this.elapsed = elapsed;
			SetFpsTo60InitiallyAndSetUsefulInitialValues();
		}

		private readonly ElapsedTime elapsed;

		private void SetFpsTo60InitiallyAndSetUsefulInitialValues()
		{
			Fps = 60;
			framesCounter = 0;
			thisFrameTicks = 0;
			lastFrameTicks = -elapsed.TicksPerSecond / Fps;
			CurrentDelta = 1.0f / Fps;
		}

		public int Fps { get; private set; }
		private int framesCounter;
		private long thisFrameTicks;
		private long lastFrameTicks;
		public float CurrentDelta { get; private set; }

		public long Milliseconds
		{
			get { return thisFrameTicks * 1000 / elapsed.TicksPerSecond; }
		}

		public void Run()
		{
			lastFrameTicks = thisFrameTicks;
			thisFrameTicks = elapsed.GetTicks();
			CurrentDelta = (float)(thisFrameTicks - lastFrameTicks) / elapsed.TicksPerSecond;
			UpdateFpsEverySecond();
		}

		private void UpdateFpsEverySecond()
		{
			framesCounter++;
			if (!CheckEvery(1.0f))
				return;

			Fps = framesCounter;
			framesCounter = 0;
		}

		public bool CheckEvery(float timeStepInSeconds)
		{
			var stepInTicks = (long)(elapsed.TicksPerSecond * timeStepInSeconds);
			if (stepInTicks <= 0)
				return true;

			return thisFrameTicks / stepInTicks > lastFrameTicks / stepInTicks;
		}

		/// <summary>
		/// Returns an accurate seconds float value for today, would get inaccurate with more days.
		/// </summary>
		public float GetSecondsSinceStartToday()
		{
			long ticksInADay = elapsed.TicksPerSecond * 60 * 60 * 24;
			long ticksToday = elapsed.GetTicks() % ticksInADay;
			return ((float)ticksToday / elapsed.TicksPerSecond);
 		}
	}
}