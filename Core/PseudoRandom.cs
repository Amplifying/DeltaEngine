using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Returns quick random integers and floats (faster than System.Random). For more info see:
	/// http://www.codeproject.com/Articles/25172/Simple-Random-Number-Generation
	/// which is based on: http://www.bobwheeler.com/statistics/Password/MarsagliaPost.txt
	/// </summary>
	public static class PseudoRandom
	{
		/// <summary>
		/// Returns an integer greater than or equal to min and strictly less than max
		/// </summary>
		public static int Get(int min, int max)
		{
			return (int)(GetNextDouble() * (max - min) + min);
		}

		private static double GetNextDouble()
		{
			if (seed == 0)
				InitializeSeed();
			uint u = GetUint();
			return (u + 1.0) * 2.328306435454494e-10;
		}

		private static long seed;

		private static void InitializeSeed()
		{
			seed = Environment.TickCount;
			var u = (uint)(seed >> 16);
			var v = (uint)(seed % 4294967296);
			w = u == 0 ? 521288629 : u;
			z = v == 0 ? 362436069 : v;
		}

		private static uint w;
		private static uint z;

		private static uint GetUint()
		{
			z = 36969 * (z & 65535) + (z >> 16);
			w = 18000 * (w & 65535) + (w >> 16);
			return (z << 16) + w;
		}

		public static float Get(float min, float max)
		{
			return (float)(GetNextDouble() * (max - min) + min);
		}
	}
}