using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlProcessor
	{
		public ControlProcessor(UIEditorViewModel uiEditorViewModel)
		{
			this.uiEditorViewModel = uiEditorViewModel;
			OutLines = new Line2D[4];
			CreateOutlines();
		}

		private readonly UIEditorViewModel uiEditorViewModel;

		private void CreateOutlines()
		{
			OutLines[0] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
			OutLines[1] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
			OutLines[2] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
			OutLines[3] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
		}

		public Line2D[] OutLines { get; private set; }
		private static readonly Color SelectionColor = Color.Red;

		internal void UpdateOutLines(Entity2D selectedSprite)
		{
			if (selectedSprite == null)
			{			
				return;
			}
			ClearLines();
			var drawArea = selectedSprite.DrawArea;
			OutLines[0].StartPoint = drawArea.TopLeft;
			OutLines[0].EndPoint = drawArea.TopRight;
			OutLines[1].StartPoint = drawArea.TopLeft;
			OutLines[1].EndPoint = drawArea.BottomLeft;
			OutLines[2].StartPoint = drawArea.BottomLeft;
			OutLines[2].EndPoint = drawArea.BottomRight;
			OutLines[3].StartPoint = drawArea.BottomRight;
			OutLines[3].EndPoint = drawArea.TopRight;
		}

		private void ClearLines()
		{
			OutLines[0].StartPoint = new Vector2D(0, 0);
			OutLines[0].EndPoint = new Vector2D(0, 0);
			OutLines[1].StartPoint = new Vector2D(0, 0);
			OutLines[1].EndPoint = new Vector2D(0, 0);
			OutLines[2].StartPoint = new Vector2D(0, 0);
			OutLines[2].EndPoint = new Vector2D(0, 0);
			OutLines[3].StartPoint = new Vector2D(0, 0);
			OutLines[3].EndPoint = new Vector2D(0, 0);
		}

		internal void MoveImage(Vector2D mousePosition, Entity2D selectedEntity2D, bool isDragging,
			bool isSnappingToGrid)
		{
			if (selectedEntity2D == null || isDragging)
				return; //ncrunch: no coverage 
			if (uiEditorViewModel.GridWidth == 0 || uiEditorViewModel.GridHeight == 0 ||
				!isSnappingToGrid)
				MoveImageWithoutGrid(mousePosition, selectedEntity2D);
			else
				MoveImageUsingTheGrid(mousePosition, selectedEntity2D);
			UpdateOutLines(selectedEntity2D);
		}

		private void MoveImageWithoutGrid(Vector2D mousePosition, Entity2D selectedEntity2D)
		{
			var relativePosition = mousePosition - lastMousePosition;
			lastMousePosition = mousePosition;
			selectedEntity2D.Center += relativePosition;
		}

		internal Vector2D lastMousePosition = Vector2D.Unused;

		private void MoveImageUsingTheGrid(Vector2D mousePosition, Entity2D selectedEntity2D)
		{
			var relativePosition = mousePosition - lastMousePosition;
			float posX = (selectedEntity2D.DrawArea.Left + relativePosition.X);
			float posY = (selectedEntity2D.DrawArea.Top + relativePosition.Y);
			float gridSpaceX = (1.0f / uiEditorViewModel.GridWidth);
			float gridSpaceY = (1.0f / uiEditorViewModel.GridHeight);
			var colomNumberInGrid = (int)Math.Round(posX / gridSpaceX);
			var rowNumberInGrid = (int)Math.Round(posY / gridSpaceY);
			selectedEntity2D.DrawArea = new Rectangle(gridSpaceX * colomNumberInGrid,
				gridSpaceY * rowNumberInGrid, selectedEntity2D.DrawArea.Width,
				selectedEntity2D.DrawArea.Height);
			if (spritePos == selectedEntity2D.Center)
				return; //ncrunch: no coverage
			lastMousePosition = mousePosition;
			spritePos = selectedEntity2D.Center;
		}

		private Vector2D spritePos;
	}
}