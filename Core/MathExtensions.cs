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

		public static bool IsNearlyEqual(this float value1, float value2, float difference = Epsilon)
		{
			return (value1 - value2).Abs() < difference;
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

		public static float Sin(float degrees)
		{
			return (float)Math.Sin(degrees * Pi / 180.0f);
		}

		public static float Cos(float degrees)
		{
			return (float)Math.Cos(degrees * Pi / 180.0f);
		}

		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2(y, x) * 180 / Pi;
		}

		public static int Clamp(this int value, int min, int max)
		{
			return value > max ? max : (value < min ? min : value);
		}

		public static float Clamp(this float value, float min, float max)
		{
			return value > max ? max : (value < min ? min : value);
		}

		public static float Lerp(float value1, float value2, float percentage)
		{
			if (percentage < 0)
				percentage = 0;
			else if (percentage > 1)
				percentage = 1;

			return value1 * (1 - percentage) + value2 * percentage;
		}

		public static float RadiansToDegrees(this float radians)
		{
			return (radians * 180.0f) / Pi;
		}

		public static float DegreesToRadians(this float degrees)
		{
			return (degrees * Pi) / 180.0f;
		}

		public static float Max(float value1, float value2)
		{
			return value1 > value2 ? value1 : value2;
		}

		public static float Min(float value1, float value2)
		{
			return value1 < value2 ? value1 : value2;
		}
	}
}