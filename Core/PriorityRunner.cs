namespace DeltaEngine.Core
{
	/// <summary>
	/// Priority runners are always executed first before all the other runners (Time, Device, etc)
	/// </summary>
	public interface PriorityRunner : Runner {}
}