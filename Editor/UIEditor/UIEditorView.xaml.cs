using System;
using System.IO.Abstractions;
using System.Windows;
using System.Windows.Controls;
using DeltaEngine.Editor.Common;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;

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
			uiEditorViewModel = new UIEditorViewModel(fileSystem);
			DataContext = uiEditorViewModel;
		}

		public UIEditorViewModel uiEditorViewModel;

		public UIEditorView(Service service)
			: this(new FileSystem()) {}

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

		public void AddImage(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("AddImage", "AddImage");
		}

		public void SelectImageInGridList(object sender, SelectionChangedEventArgs e)
		{
			EnableImageEditing(e);
			if (e.AddedItems == null)
				return;

			Messenger.Default.Send("ChangeVisualIsButton", "ChangeVisualIsButton");
			Messenger.Default.Send("ChangeVisualLayer", "ChangeVisualLayer");
			Messenger.Default.Send("ChangeVisualRotateSlider", "ChangeVisualRotateSlider");
			Messenger.Default.Send("ChangeVisualScaleSlider", "ChangeVisualScaleSlider");
		}

		private void EnableImageEditing(SelectionChangedEventArgs e)
		{
			IsButtonCheckBox.IsEnabled = e.AddedItems != null;
			LayerTextBox.IsEnabled = e.AddedItems != null;
			RotateSlider.IsEnabled = e.AddedItems != null;
			ScaleSlider.IsEnabled = e.AddedItems != null;
		}

		public void ChangeIfButtonState(object sender,
			RoutedEventArgs routedEventArgs)
		{
			if ((bool)IsButtonCheckBox.IsChecked)
				Messenger.Default.Send(true, "ChangeIsButton");
			else
				Messenger.Default.Send(false, "ChangeIsButton");
		}

		public void ChangeGridSnapping(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(GridSpacing.Text))
				return;

			ImageGrid.DrawGrid(Convert.ToInt32(GridSpacing.Text), Convert.ToInt32(GridWidthTextbox.Text),
				Convert.ToInt32(GridHeightTextbox.Text));
			Messenger.Default.Send(Convert.ToInt32(GridSpacing.Text), "ChangeGridSpacing");
		}

		public void ChangeGridWidth(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(GridWidthTextbox.Text))
				return;
			Messenger.Default.Send(Convert.ToInt32(GridWidthTextbox.Text), "ChangeGridWidth");
		}

		public void ChangeGridHeight(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(GridHeightTextbox.Text))
				return;
			Messenger.Default.Send(Convert.ToInt32(GridHeightTextbox.Text), "ChangeGridHeight");
		}

		public void LayerChanged(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(LayerTextBox.Text) || Convert.ToInt32(LayerTextBox.Text) == 0)
				return;

			Messenger.Default.Send(Convert.ToInt32(LayerTextBox.Text), "ChangeLayer");
		}

		public void RotateSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Messenger.Default.Send((int)RotateSlider.Value, "ChangeRotate");
		}

		public void ScaleSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			Messenger.Default.Send((float)ScaleSlider.Value, "ChangeScale");
		}

		public void Save(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog();
			dlg.FileName = "Document";
			dlg.DefaultExt = ".xml";
			dlg.Filter = "xml documents (.xml)|*.xml";
			bool? result = dlg.ShowDialog();
			if (result == true)
				Messenger.Default.Send(dlg.FileName, "SaveUI");
		}

		public void Load(object sender, RoutedEventArgs e)
		{
			var dlg = new OpenFileDialog();
			dlg.InitialDirectory = "c:\\";
			dlg.Filter = "xml documents (.xml)|*.xml";
			dlg.FilterIndex = 2;
			dlg.RestoreDirectory = true;
			bool? result = dlg.ShowDialog();
			if (result == true)
				Messenger.Default.Send(dlg.FileName, "LoadUI");
		}
	}
}