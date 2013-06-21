using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering
{
	/// <summary>
	/// Component holding the center of rotation for a rendered entity. 
	/// To make an entity always rotate around its own center do not have this component attached.
	/// </summary>
	public class RotationCenter
	{
		public RotationCenter(Point center)
		{
			Value = center;
		}

		public Point Value { get; set; }
	}
}