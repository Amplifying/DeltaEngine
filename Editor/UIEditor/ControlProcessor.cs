using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Shapes;
using DeltaEngine.Rendering.Sprites;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlProcessor
	{
		private readonly UIEditorViewModel uiEditorViewModel;
		public readonly Line2D[] outLines = new Line2D[4];
		private static readonly Color SelectionColor = Color.Red;
		private Point spritePos;
		internal Point lastMousePosition = Point.Unused;

		public ControlProcessor(UIEditorViewModel uiEditorViewModel)
		{
			CreateOutlines();
			this.uiEditorViewModel = uiEditorViewModel;
		}

		private void CreateOutlines()
		{
			outLines[0] = new Line2D(Point.Unused, Point.Unused, SelectionColor) { RenderLayer = 1000 };
			outLines[1] = new Line2D(Point.Unused, Point.Unused, SelectionColor) { RenderLayer = 1000 };
			outLines[2] = new Line2D(Point.Unused, Point.Unused, SelectionColor) { RenderLayer = 1000 };
			outLines[3] = new Line2D(Point.Unused, Point.Unused, SelectionColor) { RenderLayer = 1000 };
		}

		internal void UpdateOutLines(Sprite selectedSprite)
		{
			Rectangle drawArea = selectedSprite == null ? Rectangle.Zero : selectedSprite.DrawArea;
			const float Offset = 0.002f;
			outLines[0].StartPoint = drawArea.TopLeft + new Point(-Offset, -Offset);
			outLines[0].EndPoint = drawArea.TopRight + new Point(+Offset, -Offset);
			outLines[1].StartPoint = drawArea.TopLeft + new Point(-Offset, -Offset);
			outLines[1].EndPoint = drawArea.BottomLeft + new Point(-Offset, +Offset);
			outLines[2].StartPoint = drawArea.BottomLeft + new Point(-Offset, +Offset);
			outLines[2].EndPoint = drawArea.BottomRight + new Point(+Offset, +Offset);
			outLines[3].StartPoint = drawArea.BottomRight + new Point(+Offset, +Offset);
			outLines[3].EndPoint = drawArea.TopRight + new Point(+Offset, -Offset);
		}

		internal void MoveImage(Point mousePosition, Sprite selectedSprite)
		{
			if (selectedSprite == null)
				return;
			if (uiEditorViewModel.GridWidth == 0 || uiEditorViewModel.GridHeight == 0)
				MoveImageWithoutGrid(mousePosition, selectedSprite);
			else
				MoveImageUsingTheGrid(mousePosition, selectedSprite);
			UpdateOutLines(selectedSprite);
		}

		private void MoveImageWithoutGrid(Point mousePosition, Sprite selectedSprite)
		{
			var relativePosition = mousePosition - lastMousePosition;
			lastMousePosition = mousePosition;
			selectedSprite.Center += relativePosition;
		}

		private void MoveImageUsingTheGrid(Point mousePosition, Sprite selectedSprite)
		{
			var relativePosition = mousePosition - lastMousePosition;
			float posX = (selectedSprite.DrawArea.Left + relativePosition.X);
			float posY = (selectedSprite.DrawArea.Top + relativePosition.Y);
			float gridSpaceX = (1.0f / uiEditorViewModel.GridWidth);
			float gridSpaceY = (1.0f / uiEditorViewModel.GridHeight);
			var colomNumberInGrid = (int)Math.Round(posX / gridSpaceX);
			var rowNumberInGrid = (int)Math.Round(posY / gridSpaceY);
			selectedSprite.DrawArea = new Rectangle(gridSpaceX * colomNumberInGrid,
				gridSpaceY * rowNumberInGrid, selectedSprite.DrawArea.Width, selectedSprite.DrawArea.Height);
			if (spritePos != selectedSprite.Center)
			{
				lastMousePosition = mousePosition;
				spritePos = selectedSprite.Center;
			}
		}
	}
}