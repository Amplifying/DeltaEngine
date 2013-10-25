using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIControl
	{
		public float EntityWidth { get; set; }
		public float EntityHeight { get; set; }
		public int Index { get; set; }
		public bool isClicking;
		public string ControlName;
		public int controlLayer;
		public string contentText;
	}
}