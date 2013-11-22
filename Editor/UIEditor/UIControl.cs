using DeltaEngine.Content;
using DeltaEngine.Datatypes;
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
		public string VerticalAllignment;
		public string HorizontalAllignment;
		public float RightMargin;
		public float LeftMargin;
		public float TopMargin;
		public float BottomMargin;
		public string SelectedMaterial;
		public string SelectedHoveredMaterial;
		public string SelectedPressedMaterial;
		public string SelectedDisabledMaterial;

		public void SetControlSize(Entity2D control, Material material, UIEditorScene scene)
		{
			if (material.DiffuseMap.PixelSize.Width < 10 || material.DiffuseMap.PixelSize.Height < 10)
				return;
			if (scene.UIWidth > scene.UIHeight)
				control.DrawArea = new Rectangle(control.DrawArea.TopLeft,
					new Size(((material.DiffuseMap.PixelSize.Width / scene.UIWidth)),
						((material.DiffuseMap.PixelSize.Height / scene.UIWidth))));
			else
				control.DrawArea = new Rectangle(control.DrawArea.TopLeft,
					new Size(((material.DiffuseMap.PixelSize.Width / scene.UIHeight)),
						((material.DiffuseMap.PixelSize.Height / scene.UIHeight))));
		}
	}
}