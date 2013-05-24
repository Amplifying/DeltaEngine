using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeltaEngine.Editor.Common;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ContentManagerView : EditorPluginView
	{
		public ContentManagerView(Service service)
			: this(new ContentManagerViewModel(service.Content)) {}

		public ContentManagerView(ContentManagerViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			ScrollViewer.ScrollChanged += OnScrollViewerScrollChanged;
			ScrollViewer.MouseLeftButtonUp += OnMouseLeftButtonUp;
			ScrollViewer.PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
			ScrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
			ScrollViewer.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
			ScrollViewer.MouseMove += OnMouseMove;
			Slider.ValueChanged += OnSliderValueChanged;
		}

		public void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (e.ExtentHeightChange == 0 && e.ExtentWidthChange == 0)
				return;

			if (!lastMousePositionOnTarget.HasValue)
				if (lastCenterPositionOnTarget.HasValue)
					SetNewTargetCenter();
				else
					SetMousePosition();
			if (targetBefore.HasValue)
				SetNewOffset(e);
		}

		public Point? lastCenterPositionOnTarget;
		public Point? targetBefore;
		private Point? lastMousePositionOnTarget;

		private void SetNewTargetCenter()
		{
			var centerOfViewport = new Point(ScrollViewer.ViewportWidth / 2,
				ScrollViewer.ViewportHeight / 2);
			Point centerOfTargetNow = ScrollViewer.TranslatePoint(centerOfViewport, Grid);

			targetBefore = lastCenterPositionOnTarget;
			targetNow = centerOfTargetNow;
		}

		private Point? targetNow;

		private void SetMousePosition()
		{
			targetBefore = lastMousePositionOnTarget;
			targetNow = Mouse.GetPosition(Grid);

			lastMousePositionOnTarget = null;
		}

		private void SetNewOffset(ScrollChangedEventArgs e)
		{
			double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
			double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;
			double multiplicatorX = e.ExtentWidth / Grid.Width;
			double multiplicatorY = e.ExtentHeight / Grid.Height;
			double newOffsetX = ScrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
			double newOffsetY = ScrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;
			if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
				return;

			ScrollViewer.ScrollToHorizontalOffset(newOffsetX);
			ScrollViewer.ScrollToVerticalOffset(newOffsetY);
		}

		public void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ScrollViewer.Cursor = Cursors.Arrow;
			ScrollViewer.ReleaseMouseCapture();
			lastDragPoint = null;
		}

		private Point? lastDragPoint;

		public void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			lastMousePositionOnTarget = Mouse.GetPosition(Grid);
			if (e.Delta > 0)
				Slider.Value += 1;
			if (e.Delta < 0)
				Slider.Value -= 1;
			e.Handled = true;
		}

		public void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var mousePos = e.GetPosition(ScrollViewer);
			if (mousePos.X > ScrollViewer.ViewportWidth || mousePos.Y >= ScrollViewer.ViewportHeight)
				return;

			ScrollViewer.Cursor = Cursors.SizeAll;
			lastDragPoint = mousePos;
			Mouse.Capture(ScrollViewer);
		}

		public void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (!lastDragPoint.HasValue)
				return;

			Point posNow = e.GetPosition(ScrollViewer);
			double dX = posNow.X - lastDragPoint.Value.X;
			double dY = posNow.Y - lastDragPoint.Value.Y;
			lastDragPoint = posNow;
			ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - dX);
			ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - dY);
		}

		private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			ScaleTransform.ScaleX = e.NewValue;
			ScaleTransform.ScaleY = e.NewValue;

			var centerOfViewport = new Point(ScrollViewer.ViewportWidth / 2,
				ScrollViewer.ViewportHeight / 2);
			lastCenterPositionOnTarget = ScrollViewer.TranslatePoint(centerOfViewport, Grid);
		}

		private void OnImageViewDrop(object sender, DragEventArgs e)
		{
			IDataObject dataObject = e.Data;
			Messenger.Default.Send(dataObject, "AddImage");
		}

		private void DeleteSelectedImage(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("DeleteImage", "DeletingImage");
		}

		private void AddProject(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("AddProject", "AddProject");
		}

		private void ChangedSize(object sender, SizeChangedEventArgs e)
		{
			Size size = e.NewSize;
			Messenger.Default.Send(size, "ChangeImageSize");
		}

		public string ShortName
		{
			get { return "Content Manager"; }
		}
		public string Icon
		{
			get { return "Icons/Content.png"; }
		}
		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Content; }
		}
		public int Priority
		{
			get { return 1; }
		}
	}
}