using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Cameras
{
	/// <summary>
	/// 3D camera that support setting of position and target.
	/// </summary>
	public class LookAtCamera : BaseCamera
	{
		public LookAtCamera()
			: this(DefaultLookAtCamPosition, DefaultTargetPosition) { }

		public static readonly Vector DefaultLookAtCamPosition = Vector.One * 4;// new Vector(0, 4, -3);
		public static readonly Vector DefaultTargetPosition = Vector.Zero;

		public LookAtCamera(Vector position, Vector target)
			: base(position)
		{
			Target = target;
		}

		public override void Run()
		{
			const float RotationX = 30;
			rotationY += RotationSpeed * Time.Current.Delta;
			var rotationMatrix =
				Matrix.CreateRotationX(RotationX) *
				Matrix.CreateRotationY(rotationY);
			var lookVector = new Vector(0, 0, Distance);
			Position = Matrix.TransformNormal(lookVector, rotationMatrix);
			base.Run();
		}

		protected float Distance
		{
			get { return (Position - Target).Length; }
		}

		private float rotationY;
		private const float RotationSpeed = 30;
	}
}