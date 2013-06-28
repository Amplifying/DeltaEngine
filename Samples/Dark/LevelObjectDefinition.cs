using DeltaEngine.Datatypes;

namespace Dark
{
	public class LevelObjectDefinition
	{
		public Point Position { get; set; }
		public int Rotation { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public string Type { get; set; }
		public string Category { get; set; }
		public string Filename { get; set; }
		public int Room { get; set; }
	}
}