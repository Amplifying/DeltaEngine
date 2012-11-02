using System.Diagnostics;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides ticks for the Time class via the System.Diagnostics.Stopwatch class. This class is
	/// usually the fallback if nothing else has been registered for the ElapsedTime interface.
	/// </summary>
	public class StopwatchTime : ElapsedTime
	{
		private readonly Stopwatch timer = Stopwatch.StartNew();

		public long GetTicks()
		{
			return timer.ElapsedTicks;
		}

		public long TicksPerSecond
		{
			get { return Stopwatch.Frequency; }
		}
	}
}