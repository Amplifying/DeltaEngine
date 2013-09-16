using System.Windows;
using DeltaEngine.Editor.Core;

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
		}

		public void Init(Service service)
		{
			DataContext = current = new TileEditorViewModel(service);
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
			throw new System.NotImplementedException();
		}

		private void RemoveWaveButtonClick(object sender, RoutedEventArgs e)
		{
			throw new System.NotImplementedException();
		}
	}
}