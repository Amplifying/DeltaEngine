namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides total run time and delta time for the current frame in seconds. Can easily be mocked
	/// for tests and replaced with platforms framework classes for better accuracy and speed.
	/// </summary>
	public abstract class Time : PriorityRunner
	{
		static Time()
		{
			Current = new StopwatchTime();
		}

		/// <summary>
		/// StopwatchTime by default, easy to change.
		/// </summary>
		public static Time Current { get; set; }

		protected Time()
		{
			SetFpsTo60InitiallyAndSetUsefulInitialValues();
		}

		private void SetFpsTo60InitiallyAndSetUsefulInitialValues()
		{
			Fps = 60;
			framesCounter = 0;
			thisFrameTicks = 0;
			lastFrameTicks = -TicksPerSecond / Fps;
			Delta = 1.0f / Fps;
		}

		protected abstract long TicksPerSecond { get; }
		protected abstract long GetTicks();

		public int Fps { get; private set; }
		private int framesCounter;
		private long thisFrameTicks;
		private long lastFrameTicks;
		public float Delta { get; private set; }

		public long Milliseconds
		{
			get { return thisFrameTicks * 1000 / TicksPerSecond; }
		}

		public void Run()
		{
			lastFrameTicks = thisFrameTicks;
			thisFrameTicks = GetTicks();
			Delta = (float)(thisFrameTicks - lastFrameTicks) / TicksPerSecond;
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
			var stepInTicks = (long)(TicksPerSecond * timeStepInSeconds);
			if (stepInTicks <= 0)
				return true;

			return thisFrameTicks / stepInTicks > lastFrameTicks / stepInTicks;
		}

		/// <summary>
		/// Returns an accurate seconds float value for today, would get inaccurate with more days.
		/// </summary>
		public float GetSecondsSinceStartToday()
		{
			long ticksInADay = TicksPerSecond * 60 * 60 * 24;
			long ticksToday = GetTicks() % ticksInADay;
			return ((float)ticksToday / TicksPerSecond);
		}
	}
}