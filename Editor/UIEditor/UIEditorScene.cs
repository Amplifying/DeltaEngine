using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes;
using DeltaEngine.ScreenSpaces;

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
		public List<Line2D> LinesInGridList = new List<Line2D>();
		public List<Line2D> GridOutLine = new List<Line2D>();
		public bool IsDrawingGrid { get; set; }
		public string SelectedControlNameInList { get; set; }
		public string SelectedResolution { get; set; }
		public ControlProcessor ControlProcessor { get; set; }
		public int UIWidth { get; set; }
		public int UIHeight { get; set; }

		public void UpdateOutLine(Entity2D selectedEntity2D)
		{
			ControlProcessor.UpdateOutLines(selectedEntity2D);
		}

		public void CreateGrtidOutline()
		{
			GridOutLine.Add(new Line2D(new Vector2D(0, 0), new Vector2D(0, 0), Color.Red));
			GridOutLine.Add(new Line2D(new Vector2D(0, 0), new Vector2D(0, 0), Color.Red));
			GridOutLine.Add(new Line2D(new Vector2D(0, 0), new Vector2D(0, 0), Color.Red));
			GridOutLine.Add(new Line2D(new Vector2D(0, 0), new Vector2D(0, 0), Color.Red));
		}

		public void UpdateGridOutline()
		{
			if (UIWidth <= 0 || UIHeight <= 0)
				return;
			var sceneSize = ScreenSpace.Current.FromPixelSpace(new Size(UIWidth, UIHeight));
			var xOffset = 0.5f;
			var yOffset = 0.5f;
			var aspect = sceneSize.Width / sceneSize.Height;
			float width = 0;
			float height = 0;
			if (aspect > 1)
			{
				yOffset = 1 / (2 * aspect);
				width = 1;
				height = width / aspect;
			}
			else if (aspect < 1)
			{
				xOffset = aspect / 2;
				height = 1;
				width = height * aspect;
			}
			else
			{
				height = 1;
				width = 1;
			}
			var topLeft = new Vector2D(0.5f - xOffset, 0.5f - yOffset);
			GridOutLine[0].Points[0] = topLeft;
			GridOutLine[0].Points[1] = new Vector2D(topLeft.X + width, topLeft.Y);
			GridOutLine[1].Points[0] = topLeft;
			GridOutLine[1].Points[1] = new Vector2D(topLeft.X, topLeft.Y + height);
			GridOutLine[2].Points[0] = new Vector2D(topLeft.X, topLeft.Y + height);
			GridOutLine[2].Points[1] = new Vector2D(topLeft.X + width,
				topLeft.Y + height);
			GridOutLine[3].Points[0] = new Vector2D(topLeft.X + width, topLeft.Y);
			GridOutLine[3].Points[1] = new Vector2D(topLeft.X + width,
				topLeft.Y + height);
			foreach (var line in GridOutLine)
				line.IsActive = true;
		}
	}
}