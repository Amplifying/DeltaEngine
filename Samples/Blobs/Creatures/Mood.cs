using DeltaEngine.Core;

namespace Blobs.Creatures
{
	/// <summary>
	/// Manages the mood of a Blob
	/// </summary>
	public class Mood
	{
		public float Anger
		{
			get { return anger; }
			set { anger = value.Clamp(0.0f, 1.0f); }
		}

		private float anger;

		public float Fear
		{
			get { return 1.0f - anger; }
			set { Anger = 1.0f - value; }
		}
	}
}