namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Useful for processing 3D rotations. Riemer's XNA tutorials have a nice introductory 
	/// example of use: http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Quaternions.php
	/// http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Flight_kinematics.php
	/// </summary>
	public class Quaternion
	{
		public Quaternion(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float W { get; set; }
	}
}
