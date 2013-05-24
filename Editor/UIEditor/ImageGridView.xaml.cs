using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DeltaEngine.Datatypes;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	/// <summary>
	/// Interaction logic for ImageGrid.xaml
	/// </summary>
	public partial class ImageGridView
	{
		public ImageGridView()
		{
			InitializeComponent();
		}

		private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var point = new Point();
			point.X = (float)e.GetPosition(this).X;
			point.Y = (float)e.GetPosition(this).Y;
			Messenger.Default.Send(point, "LeftMouseDown");
		}

		private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
		{
			var point = new Point();
			point.X = (float)e.GetPosition(this).X;
			point.Y = (float)e.GetPosition(this).Y;
			Messenger.Default.Send(point, "MouseMove");
		}

		private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var point = new Point();
			point.X = (float)e.GetPosition(this).X;
			point.Y = (float)e.GetPosition(this).Y;
			Messenger.Default.Send(point, "LeftMouseUp");
		}

		public void DrawGrid(int gridSize, int gridWidth, int gridHeight)
		{
			foreach (var line in lineList)
				SnappingGrid.Children.Remove(line);
			lineList.Clear();
			if (gridSize < 5)
				return;

			DrawVisualGrid(gridSize, gridWidth, gridHeight);
		}

		private void DrawVisualGrid(int gridSize, int gridWidth, int gridHeight)
		{
			float numberOfHorizontalGrid = gridWidth / (float)gridSize;
			for (int i = 0; i < numberOfHorizontalGrid; i++)
				DrawLine(gridWidth, i, numberOfHorizontalGrid, true);
			float numberOfVerticalGrid = gridHeight / (float)gridSize;
			for (int i = 0; i < numberOfVerticalGrid; i++)
				DrawLine(gridHeight, i, numberOfVerticalGrid, false);
		}

		private readonly List<Line> lineList = new List<Line>();

		private void DrawLine(int spacing, int i, float gridnumber, bool isHorizontal)
		{
			var myLine = new Line();
			myLine.Stroke = Brushes.LightSteelBlue;
			if(isHorizontal)
				SetPointsInXDirection(spacing, i, gridnumber, myLine);
			else
				SetPointsInYDirection(spacing, i, gridnumber, myLine);
			myLine.HorizontalAlignment = 0;
			myLine.VerticalAlignment = 0;
			myLine.StrokeThickness = 1;
			SnappingGrid.Children.Add(myLine);
			lineList.Add(myLine);
		}
		private static void SetPointsInXDirection(int spacing, int i, float gridnumber, Line myLine)
		{
			myLine.X1 = 0;
			myLine.X2 = spacing;
			myLine.Y1 = i * (spacing / gridnumber);
			myLine.Y2 = i * (spacing / gridnumber);
		}

		private static void SetPointsInYDirection(int gridHeight, int i, float numberOfVerticalGrid,
			Line myLine)
		{
			myLine.X1 = i * (gridHeight / numberOfVerticalGrid);
			myLine.X2 = i * (gridHeight / numberOfVerticalGrid);
			myLine.Y1 = 0;
			myLine.Y2 = gridHeight;
		}
	}
}