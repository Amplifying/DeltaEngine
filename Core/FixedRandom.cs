namespace DeltaEngine.Core
{
	public class FixedRandom : Randomizer
	{
		public FixedRandom(float[] fixedValues = null)
		{
			this.fixedValues = fixedValues;
		}

		private readonly float[] fixedValues;

		public float Get(float min = 0.0f, float max = 1.0f)
		{
			return GetNextFloat() * (max - min) + min;
		}

		private float GetNextFloat()
		{
			if (fixedValues == null || fixedValues.Length == 0)
				return 0.0f;

			return fixedValues[index++ % fixedValues.Length];
		}

		private int index;

		public int Get(int min, int max)
		{
			return (int)Get((float)min, max);
		}
	}
}