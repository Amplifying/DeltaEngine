namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Interval of two values; Allows a random value in between to be obtained.
	/// </summary>
	public struct Range<T> where T:Lerp<T>
	{
		public Range(T minimum, T maximum)
			: this()
		{
			Start = minimum;
			End = maximum;
		}

		public T Start { get; set; }
		public T End { get; set; }


		public T GetRandomValue()
		{
			return Start.Lerp(End, Randomizer.Current.Get());
		}
	}
}
