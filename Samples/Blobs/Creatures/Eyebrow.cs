using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;

namespace Blobs.Creatures
{
	/// <summary>
	/// Controls the eyebrow of a Blob
	/// </summary>
	public abstract class Eyebrow : Line2D, Runner
	{
		protected Eyebrow(Eye eye, Mood mood)
			: base(Point.Zero, Point.Zero, Color.Black)
		{
			this.eye = eye;
			this.mood = mood;
			RenderLayer = 11;
		}

		protected readonly Eye eye;
		protected readonly Mood mood;

		public virtual void Run()
		{
			StartPoint.RotateAround(eye.Center, eye.Rotation);
			EndPoint.RotateAround(eye.Center, eye.Rotation);
		}
	}
}