using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Navigation;

namespace DeltaEngine.Editor.ParticleEditor
{
	public partial class GraphControl
	{
		public GraphControl()
		{
			InitializeComponent();
			DataPoints = new PointCollection();
			//DataPoints.Add(new Point(0.0f, 0.1f));
			//DataPoints.Add(new Point(0.25f, 0.7f));
			//DataPoints.Add(new Point(0.5f, 1.0f));
			//DataPoints.Add(new Point(0.5f, 1.0f));
			//DataPoints.Add(new Point(0.25f, 0.4f));
			//DataPoints.Add(new Point(0, 0.0f));
			Loaded += OnLoaded;
		}

		public PointCollection DataPoints { get; set; }

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			SetupPlotter();
			AddCurrentRunningTimeLine();
			CreateLineGraph();
		}

		private void SetupPlotter()
		{
			Plotter.MouseLeftButtonDown += PlotterMouseLeftButtonDown;
			Plotter.MouseLeftButtonUp += PlotterOnMouseLeftButtonUp;
			Plotter.AddChild(new CursorCoordinateGraph { LineStrokeThickness = 0.4 });
			Plotter.Viewport.Visible = new DataRect(0, 0, 1, 1);
			Plotter.Viewport.ClipToBoundsEnlargeFactor = 1.0f;
			SetupRemovePointInContextMenu();
		}

		private void PlotterMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
		{
			remStartClickPosition = args.GetPosition(Plotter);
		}

		private Point remStartClickPosition;

		private void PlotterOnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
		{
			if (args.ClickCount == 1 && args.GetPosition(Plotter) == remStartClickPosition)
				AddNewPoint(ToDataPosition(remStartClickPosition));
		}

		private void AddNewPoint(Point newPoint)
		{
			if (FindNearestPointOnlyX(newPoint, 0.025f) ||
				DataPoints.Count > 0 && newPoint.X < DataPoints[0].X)
				return;
			for (int num = 1; num < DataPoints.Count; num++)
				if (newPoint.X < DataPoints[num].X)
				{
					DataPoints.Insert(num, newPoint);
					return;
				}
			DataPoints.Add(newPoint);
		}

		private bool FindNearestPointOnlyX(Point dataPosition, float distance)
		{
			float smallestDistance = distance;
			foreach (var point in DataPoints)
			{
				var xDiff = (float)Math.Abs(point.X - dataPosition.X);
				if (xDiff >= smallestDistance)
					continue;
				smallestDistance = xDiff;
			}
			return smallestDistance < distance;
		}

		private bool FindNearestPoint(Point dataPosition, float distance)
		{
			float smallestSquareDistance = distance * distance;
			foreach (var point in DataPoints)
			{
				var xDiff = (float)(point.X - dataPosition.X);
				var yDiff = (float)(point.Y - dataPosition.Y);
				var squareDistance = xDiff * xDiff + yDiff * yDiff;
				if (squareDistance >= smallestSquareDistance)
					continue;
				smallestSquareDistance = squareDistance;
				nearestPoint = point;
			}
			return smallestSquareDistance < distance * distance;
		}

		private Point nearestPoint;

		private Point ToDataPosition(Point plotterScreenPosition)
		{
			plotterScreenPosition.X -= Plotter.MainVerticalAxis.ActualWidth;
			return Plotter.Transform.ScreenToData(plotterScreenPosition);
		}

		private void SetupRemovePointInContextMenu()
		{
			var removePoint = new MenuItem
			{
				Header = "Remove Point",
				ToolTip = "Remove nearest point",
				Icon = new Image { Source = DefaultContextMenu.LoadIcon("RemoveIcon") },
				Command = new RelayCommand(RemovePoint, IsContextMenuPointNearby),
				CommandTarget = Plotter
			};
			var keyBinding = new KeyBinding(new RelayCommand(RemovePoint, IsMousePointNearby), Key.Delete,
				ModifierKeys.None);
			InputBindings.Add(keyBinding);
			removePoint.InputGestureText = "Del";
			Plotter.DefaultContextMenu.StaticMenuItems.Insert(0, removePoint);
		}

		private void RemovePoint()
		{
			if (nearestPoint != new Point())
				DataPoints.Remove(nearestPoint);
			nearestPoint = new Point();
		}

		private bool IsContextMenuPointNearby()
		{
			return FindNearestPointFromScreenPoint(Plotter.DefaultContextMenu.MousePositionOnClick);
		}

		private bool IsMousePointNearby()
		{
			return FindNearestPointFromScreenPoint(Mouse.GetPosition(Plotter));
		}

		private bool FindNearestPointFromScreenPoint(Point clickPosition)
		{
			return FindNearestPoint(ToDataPosition(clickPosition), 0.15f);
		}

		private void AddCurrentRunningTimeLine()
		{
			Plotter.AddChild(new VerticalLine(0.5f)
			{
				StrokeThickness = 0.75,
				Stroke = new SolidColorBrush(Colors.LightSkyBlue)
			});
		}

		private void CreateLineGraph()
		{
			editor = new PolylineEditor();
			editor.Polyline = new ViewportPolyline();
			//could be helpful for range: editor.Polyline.Fill = new SolidColorBrush(Colors.Violet);
			editor.Polyline.Stroke = new SolidColorBrush(Colors.Green);
			editor.Polyline.StrokeThickness = 1;
			editor.Polyline.Points = DataPoints;
			//editor.Polyline.to
			LiveToolTipService.SetToolTip(editor.Polyline,
				new LiveToolTip { Background = Brushes.Green, Content = "X" });
			/*
		<d3:LiveToolTipService.ToolTip>
				<d3:LiveToolTip Background="Violet">
					PolyBezier line with control points editing
				</d3:LiveToolTip>
			</d3:LiveToolTipService.ToolTip>
			 */
			editor.AddToPlotter(Plotter);
		}

		private PolylineEditor editor;
	}
}