namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides Ticks for the Time class to measure time differences between frames. Can easily be
	/// mocked for tests and replaced with platforms framework classes for better accuracy and speed.
	/// </summary>
	public interface ElapsedTime
	{
		long GetTicks();
		long TicksPerSecond { get; }
	}
}