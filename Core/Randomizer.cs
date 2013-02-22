namespace DeltaEngine.Core
{
	/// <summary>
	/// The definition for all random number generators
	/// </summary>
	public interface Randomizer
	{
		/// <summary>
		/// Returns a float between min and max, by default a value between zero and one.
		/// </summary>
		float Get(float min = 0, float max = 1);
		/// <summary>
		/// Returns an integer greater than or equal to min and strictly less than max
		/// </summary>
		int Get(int min, int max);
	}
}
