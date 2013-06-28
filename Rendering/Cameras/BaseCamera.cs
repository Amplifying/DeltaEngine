using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering.Cameras
{
	/// <summary>
	/// Abstract base camera class, provides some useful constants and properties
	/// that are commonly used in most camera classes. Is used as a dynamic
	/// module which updates itself directly after the input module updated, so
	/// the camera always have the newest input data to use for camera movement.
	/// </summary>
	public abstract class BaseCamera : Runner
	{
		protected BaseCamera(Vector position)
		{
			FieldOfView = DefaultFieldOfView;
			FarPlane = DefaultFarPlane;
			NearPlane = DefaultNearPlane;
			Position = position;
		}

		public float FieldOfView { get; set; }
		public const float DefaultFieldOfView = 45.0f;
		public float FarPlane { get; set; }
		public const float DefaultFarPlane = 120.0f;
		public float NearPlane { get; set; }
		public const float DefaultNearPlane = 0.45f;
		public Vector Position { get; set; }

		public virtual void Run()
		{
			UpdateViewMatrix();
		}

		protected virtual void UpdateViewMatrix()
		{
			cameraViewMatrix = Matrix.CreateLookAt(Position, Target, UpVector);
		}

		protected Matrix cameraViewMatrix = Matrix.Identity;
		public Vector Target { get; set; }
		public static readonly Vector UpVector = Vector.UnitY;

		public Matrix ViewMatrix
		{
			get
			{
				return cameraViewMatrix;
			}
		}
	}
}