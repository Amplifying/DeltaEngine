using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIEditorScene
	{
		public int GridWidth { get; set; }
		public int GridHeight { get; set; }
		public ObservableCollection<string> ContentImageListList { get; set; }
		public ObservableCollection<string> UIImagesInList { get; set; }
		public ObservableCollection<string> MaterialList { get; set; }
		public ObservableCollection<string> SceneNames { get; set; }
		public ObservableCollection<string> ResolutionList { get; set; }
		public Scene Scene { get; set; }
		public Entity2D SelectedEntity2D { get; set; }
		public bool IsSnappingToGrid { get; set; }
		public string UIName { get; set; }
		public List<Line2D> linesInGridList = new List<Line2D>();
		public bool IsDrawingGrid { get; set; }
		public string SelectedSpriteNameInList { get; set; }
		public string SelectedResolution { get; set; }
		public ControlProcessor ControlProcessor { get; set; }

		public void UpdateOutLine(Entity2D selectedEntity2D)
		{
			ControlProcessor.UpdateOutLines(selectedEntity2D);
		}
	}
}