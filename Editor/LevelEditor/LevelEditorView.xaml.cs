using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;
using DeltaEngine.GameLogic;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.LevelEditor
{
	public partial class LevelEditorView : EditorPluginView
	{
		public LevelEditorView()
		{
			InitializeComponent();
			SizeComboBox.ItemsSource = new[]
			{
				new Size(8, 8), new Size(12, 12), new Size(16, 16), new Size(24, 24), new Size(32, 32),
				new Size(64, 64), new Size(128, 128)
			};
		}

		public void Init(Service service)
		{
			DataContext = viewModel = new LevelEditorViewModel(service);
			ContentNameTextBox.ItemsSource = service.GetAllContentNamesByType(ContentType.Level);
		}

		private LevelEditorViewModel viewModel;

		public void ProjectChanged() {}

		private void OpenXmlFileButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.OpenXmlFile();
		}

		private void SaveButtonClicked(object sender, RoutedEventArgs e)
		{
			viewModel.SaveToServer();
		}

		private void AddWaveButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.AddWave(viewModel.WaitTime, viewModel.SpawnInterval, viewModel.MaxTime,
				viewModel.SpawnTypeList);
		}

		private void RemoveWaveButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.RemoveSelectedWave();
		}

		private void WavesListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			viewModel.SelectedWave = WavesListBox.SelectedItem as Wave;
		}

		public string ShortName
		{
			get { return "Level Editor"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Level.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		private void AddDecal(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				viewModel.SetDragingDecal(true);
				isDragging = true;
				Mouse.OverrideCursor = Cursors.Hand;
			}
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				viewModel.SetDragingDecal(false);
				isDragging = false;
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private bool isDragging;
	}
}