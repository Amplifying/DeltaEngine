using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Returns quick random integers and floats (faster than System.Random). For more info see:
	/// http://www.codeproject.com/Articles/25172/Simple-Random-Number-Generation
	/// which is based on: http://www.bobwheeler.com/statistics/Password/MarsagliaPost.txt
	/// </summary>
	public class PseudoRandom : Randomizer
	{
		public PseudoRandom()
		{
			seed = Environment.TickCount;
			var u = (uint)(seed >> 16);
			var v = (uint)(seed % 4294967296);
			w = u == 0 ? 521288629 : u;
			z = v == 0 ? 362436069 : v;
		}

		private readonly long seed;
		private uint w;
		private uint z;

		public int Get(int min, int max)
		{
			return (int)(GetNextDouble() * (max - min) + min);
		}

		private double GetNextDouble()
		{
			uint u = GetUint();
			return (u + 1.0) * 2.328306435454494e-10;
		}

		private uint GetUint()
		{
			z = 36969 * (z & 65535) + (z >> 16);
			w = 18000 * (w & 65535) + (w >> 16);
			return (z << 16) + w;
		}

		public float Get(float min, float max)
		{
			return (float)(GetNextDouble() * (max - min) + min);
		}
	}
}