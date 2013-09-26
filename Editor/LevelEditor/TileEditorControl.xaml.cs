using System.Windows;
using System.Windows.Controls;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;
using DeltaEngine.Rendering2D.Shapes.Levels;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.TileEditor
{
	/// <summary>
	/// Interaction logic for TileEditorView.xaml
	/// </summary>
	public partial class TileEditorControl : EditorPluginView
	{
		public TileEditorControl()
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
			DataContext = current = new TileEditorViewModel(service);
			ContentNameTextBox.ItemsSource = service.GetAllContentNamesByType(ContentType.Level);
		}

		private TileEditorViewModel current;

		public string ShortName
		{
			get { return "Tile Editor"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Level.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		private void OpenXmlFileButtonClick(object sender, RoutedEventArgs e)
		{
			current.OpenXmlFile();
		}

		private void SaveButtonClicked(object sender, RoutedEventArgs e)
		{
			current.SaveToServer();
		}

		private void AddWaveButtonClick(object sender, RoutedEventArgs e)
		{
			current.AddWave(current.WaitTime, current.SpawnInterval, current.MaxTime,
				current.SpawnTypeList);
		}

		private void RemoveWaveButtonClick(object sender, RoutedEventArgs e)
		{
			current.RemoveSelectedWave();
		}

		private void WavesListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			current.SelectedWave = WavesListBox.SelectedItem as Wave;
		}
	}
}