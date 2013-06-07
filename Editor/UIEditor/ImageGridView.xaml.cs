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

		private void OnLeftMouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			var point = new Point();
			point.X = (float)e.GetPosition(this).X;
			point.Y = (float)e.GetPosition(this).Y;
			Messenger.Default.Send(point, "LeftMouseDown");
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			var point = new Point();
			point.X = (float)e.GetPosition(this).X;
			point.Y = (float)e.GetPosition(this).Y;
			Messenger.Default.Send(point, "MouseMove");
		}

		private void OnLeftMouseButtonUp(object sender, MouseButtonEventArgs e)
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
			float numberOfHorizontalGrid = gridHeight / (float)gridSize;
			for (int i = 0; i < numberOfHorizontalGrid; i++)
				DrawLine(gridHeight, gridWidth, i, numberOfHorizontalGrid, true);
			float numberOfVerticalGrid = gridWidth / (float)gridSize;
			for (int i = 0; i < numberOfVerticalGrid; i++)
				DrawLine(gridWidth, gridHeight, i, numberOfVerticalGrid, false);
		}

		private readonly List<Line> lineList = new List<Line>();

		private void DrawLine(int gridWidth, int gridHeight, int i, float gridnumber,
			bool isHorizontal)
		{
			var gridLine = new Line();
			Brush brush = Brushes.LightSteelBlue;
			gridLine.Stroke = brush;
			if (isHorizontal)
				SetPointsInXDirection(gridWidth, i, gridnumber, gridLine, gridHeight);
			else
				SetPointsInYDirection(gridWidth, i, gridnumber, gridLine, gridHeight);
			gridLine.HorizontalAlignment = 0;
			gridLine.VerticalAlignment = 0;
			gridLine.StrokeThickness = 1;
			SnappingGrid.Children.Add(gridLine);
			lineList.Add(gridLine);
		}

		private static void SetPointsInXDirection(int spacing, int i, float gridnumber, Line gridLine,
			int gridHeight)
		{
			gridLine.X1 = 0;
			gridLine.X2 = gridHeight;
			gridLine.Y1 = i * (spacing / gridnumber);
			gridLine.Y2 = i * (spacing / gridnumber);
		}

		private static void SetPointsInYDirection(int spacing, int i, float gridNumber, Line gridLine,
			int gridHeight)
		{
			gridLine.X1 = i * (spacing / gridNumber);
			gridLine.X2 = i * (spacing / gridNumber);
			gridLine.Y1 = 0;
			gridLine.Y2 = gridHeight;
		}
	}
}