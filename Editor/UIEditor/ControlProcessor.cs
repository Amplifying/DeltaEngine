using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlProcessor
	{
		private readonly UIEditorViewModel uiEditorViewModel;
		public readonly Line2D[] outLines = new Line2D[4];
		private static readonly Color SelectionColor = Color.Red;
		private Vector2D spritePos;
		internal Vector2D lastMousePosition = Vector2D.Unused;

		public ControlProcessor(UIEditorViewModel uiEditorViewModel)
		{
			CreateOutlines();
			this.uiEditorViewModel = uiEditorViewModel;
		}

		private void CreateOutlines()
		{
			outLines[0] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
			outLines[1] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
			outLines[2] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
			outLines[3] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
		}

		internal void UpdateOutLines(Entity2D selectedSprite)
		{
			Rectangle drawArea = selectedSprite == null ? Rectangle.Zero : selectedSprite.DrawArea;
			const float Offset = 0.002f;
			outLines[0].StartPoint = drawArea.TopLeft + new Vector2D(-Offset, -Offset);
			outLines[0].EndPoint = drawArea.TopRight + new Vector2D(+Offset, -Offset);
			outLines[1].StartPoint = drawArea.TopLeft + new Vector2D(-Offset, -Offset);
			outLines[1].EndPoint = drawArea.BottomLeft + new Vector2D(-Offset, +Offset);
			outLines[2].StartPoint = drawArea.BottomLeft + new Vector2D(-Offset, +Offset);
			outLines[2].EndPoint = drawArea.BottomRight + new Vector2D(+Offset, +Offset);
			outLines[3].StartPoint = drawArea.BottomRight + new Vector2D(+Offset, +Offset);
			outLines[3].EndPoint = drawArea.TopRight + new Vector2D(+Offset, -Offset);
		}

		internal void MoveImage(Vector2D mousePosition, Entity2D selectedEntity2D, bool isDragging,
			bool isSnappingToGrid)
		{
			if (selectedEntity2D == null || isDragging)
				return;
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
				return;
			lastMousePosition = mousePosition;
			spritePos = selectedEntity2D.Center;
		}
	}
}