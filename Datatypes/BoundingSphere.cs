namespace DeltaEngine.Datatypes
{
	public class BoundingSphere
	{
		public BoundingSphere(Vector center, float radius)
		{
			Center = center;
			Radius = radius;
		}

		public Vector Center { get; set; }
		public float Radius { get; set; }
	}
}
