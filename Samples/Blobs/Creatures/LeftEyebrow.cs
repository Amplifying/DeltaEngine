using DeltaEngine.Datatypes;

namespace Blobs.Creatures
{
	/// <summary>
	/// Controls the left eyebrow of a Blob
	/// </summary>
	public class LeftEyebrow : Eyebrow
	{
		public LeftEyebrow(Eye eye, Mood mood)
			: base(eye, mood) {}

		public override void Run()
		{
			StartPoint = eye.Center -
				new Point(eye.RadiusX, (1.5f + (0.5f * mood.Anger) - (0.25f * mood.Anger)) * eye.RadiusY);
			EndPoint = eye.Center +
				new Point(eye.RadiusX, (-2.0f + (0.5f * mood.Anger) + (0.25f * mood.Anger)) * eye.RadiusY);
		}
	}
}