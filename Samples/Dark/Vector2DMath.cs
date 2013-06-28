using DeltaEngine.Datatypes;

namespace Dark
{
	public static class Vector2DMath
	{
		public static float GetSquaredLength(Vector vector)
		{
			return vector.X * vector.X + vector.Y * vector.Y;
		}

		public static float GetLength(Vector vector)
		{
			return (float)System.Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
		}

		public static Vector Normalize(Vector vector)
		{
			float length = GetLength(vector);
			return new Vector(vector.X / length, vector.Y / length, 0.0f);
		}

		public static Vector GetDirection(Point from, Point to)
		{
			return Normalize(new Vector(to.X - from.X, to.Y - from.Y, 0.0f));
		}

		public static float GetDotProduct(Vector a, Vector b)
		{
			return a.X * b.X + a.Y * b.Y;
		}

		public static float GetAngle(Vector a, Vector b)
		{
			return (float)(System.Math.Atan2(b.Y, b.X) - System.Math.Atan2(a.Y, a.X));
		}
	}
}