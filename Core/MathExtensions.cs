using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Extends the System.Math class, but uses floats and provides some extra constants.
	/// </summary>
	public static class MathExtensions
	{
		public const float Pi = 3.14159265359f;

		public static float Abs(this float value)
		{
			return Math.Abs(value);
		}

		public static bool IsNearlyEqual(this float value1, float value2)
		{
			return (value1 - value2).Abs() < Epsilon;
		}

		private const float Epsilon = 0.0001f;

		public static int Round(float value)
		{
			return (int)Math.Round(value);
		}

		public static float Round(float value, int decimals)
		{
			return (float)Math.Round(value, decimals);
		}

		public static float Sin(float degreeValue)
		{
			return (float)Math.Sin(degreeValue * Pi / 180.0f);
		}

		public static float Cos(float degreeValue)
		{
			return (float)Math.Cos(degreeValue * Pi / 180.0f);
		}

		public static float Lerp(float value1, float value2, float percentage)
		{
			if (percentage < 0)
				percentage = 0;
			else if (percentage > 1)
				percentage = 1;

			return value1 * (1 - percentage) + value2 * percentage;
		}
	}
}