using System;
using System.IO.Abstractions;
using System.Windows;
using System.Windows.Controls;
using DeltaEngine.Editor.Common;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	/// <summary>
	/// Interaction logic for UIEditor.xaml
	/// </summary>
	public partial class UIEditorView : EditorPluginView
	{
		public UIEditorView(IFileSystem fileSystem)
		{
			InitializeComponent();
			DataContext = new UIEditorViewModel(fileSystem);
		}

		public UIEditorView(Service service)
			: this(new FileSystem()) { }

		public string ShortName
		{
			get { return "UI Editor"; }
		}

		public string Icon
		{
			get { return "Icons/UI.png"; }
		}

		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Content; }
		}

		public int Priority
		{
			get { return 1; }
		}

		private void AddImage(object sender, System.Windows.RoutedEventArgs e)
		{
			Messenger.Default.Send("AddImage", "AddImage");
		}

		private void SelectImageInGridList(object sender, SelectionChangedEventArgs e)
		{
			IsButtonCheckBox.IsEnabled = e.AddedItems != null;
			if (e.AddedItems == null)
				return;

			Messenger.Default.Send("ChangeVisualIsButton", "ChangeVisualIsButton");
			Messenger.Default.Send("ChangeVisualLayer", "ChangeVisualLayer");
			Messenger.Default.Send("ChangeVisualRotateSlider", "ChangeVisualRotateSlider");
			Messenger.Default.Send("ChangeVisualScaleSlider", "ChangeVisualScaleSlider");
		}

		private void ChangeIfButtonStateTrue(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(true, "ChangeIsButton");
		}

		private void ChangeIfButtonStateFalse(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(false, "ChangeIsButton");
		}

		private void ChangeGridSnapping(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(GridSpacing.Text))
				return;

			ImageGrid.DrawGrid(Convert.ToInt32(GridSpacing.Text), Convert.ToInt32(GridWidthTextbox.Text),
				Convert.ToInt32(GridHeightTextbox.Text));
			Messenger.Default.Send(Convert.ToInt32(GridSpacing.Text), "ChangeGridSpacing");
		}

		private void ChangeGridWidth(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(GridWidthTextbox.Text))
				return;
			Messenger.Default.Send(Convert.ToInt32(GridWidthTextbox.Text), "ChangeGridWidth");
		}

		private void ChangeGridHeight(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(GridHeightTextbox.Text))
				return;
			Messenger.Default.Send(Convert.ToInt32(GridHeightTextbox.Text), "ChangeGridHeight");
		}

		private void LayerChanged(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(LayerTextBox.Text) || Convert.ToInt32(LayerTextBox.Text) == 0)
				return;

			Messenger.Default.Send(Convert.ToInt32(LayerTextBox.Text), "ChangeLayer");
		}

		private void RotateSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Messenger.Default.Send((int)RotateSlider.Value, "ChangeRotate");
		}

		private void ScaleSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Messenger.Default.Send((float)ScaleSlider.Value, "ChangeScale");
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("SaveUI", "SaveUI");
		}

		private void Load(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("LoadUI", "LoadUI");
		}
	}
}