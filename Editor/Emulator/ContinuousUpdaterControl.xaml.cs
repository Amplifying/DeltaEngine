using System.Windows;
using AurelienRibon.Ui.SyntaxHighlightBox;
using DeltaEngine.Editor.Core;
using Microsoft.Win32;

namespace DeltaEngine.Editor.Emulator
{
	/// <summary>
	/// Interaction logic for ContinuousUpdaterControl.xaml
	/// </summary>
	public partial class ContinuousUpdaterControl : EditorPluginView
	{
		public ContinuousUpdaterControl()
		{
			InitializeComponent();
			SourceCodeTextBox.CurrentHighlighter = HighlighterManager.Instance.Highlighters["CSharp"];
		}

		public void Init(Service service)
		{
			DataContext = viewModel = new ContinuousUpdaterViewModel(service);
		}

		public void ProjectChanged() {}

		private ContinuousUpdaterViewModel viewModel;


		private void StopUpdatingButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.StopUpdating();
		}

		private void Restart(object sender, RoutedEventArgs e)
		{
			viewModel.Restart();
		}

		private void OpenInVisualStudioButton(object sender, RoutedEventArgs e)
		{
			viewModel.OpenCurrentTestInVisualStudio();
		}

		private void SelectAssemblyFilePathButtonClick(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				DefaultExt = ".dll;.exe",
				Filter = "C# Project Assembly (.dll, .exe)|*.dll;*.exe",
				InitialDirectory = InitialDirectoryForBrowseDialog
			};
			if (dialog.ShowDialog().Equals(true))
				viewModel.AssemblyFilePath = new AssemblyFile(dialog.FileName);
		}

		private const string InitialDirectoryForBrowseDialog = "";

		public string ShortName
		{
			get { return "Continuous Updater"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Start.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}
	}
}