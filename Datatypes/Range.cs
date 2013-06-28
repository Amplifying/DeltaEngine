using System;
using DeltaEngine.Core;

namespace DeltaEngine.Datatypes
{
	public struct Range
	{
		public Range(float minimum, float maximum)
			: this()
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		public float Minimum { get; set; }
		public float Maximum { get; set; }

		public Range(string sizeAsString)
			: this()
		{
			float[] components = sizeAsString.SplitIntoFloats();
			if (components.Length != 2)
				throw new InvalidNumberOfComponents();

			Minimum = components[0];
			Maximum = components[1];
		}

		public class InvalidNumberOfComponents : Exception { }

		public float GetRandomValue()
		{
			return Randomizer.Current.Get(Minimum, Maximum);
		}
	}
}
