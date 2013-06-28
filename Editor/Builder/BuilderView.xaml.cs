using System.Windows;
using DeltaEngine.Editor.Common;
using Microsoft.Win32;

namespace DeltaEngine.Editor.Builder
{
	/// <summary>
	/// Interaction logic for LauncherView.xaml
	/// </summary>
	public partial class BuilderView : EditorPluginView
	{
		public BuilderView(Service service)
			: this(new BuilderViewModel(service)) {}

		public BuilderView(BuilderViewModel viewModel)
		{
			InitializeComponent();
			this.viewModel = viewModel;
			DataContext = viewModel;
			BuildList.MessagesViewModel = viewModel.MessagesListViewModel;
			BuildList.AppListViewModel = viewModel.AppListViewModel;
			BuildList.FocusBuiltAppsList();
			TrySelectEngineSamplesSolution();
		}

		private readonly BuilderViewModel viewModel;

		private void TrySelectEngineSamplesSolution()
		{
			try
			{
				viewModel.SelectSamplesSolution();
			}
			catch (BuilderViewModel.DeltaEnginePathUnknown)
			{
				MessageBox.Show(viewModel.Service.PluginHostWindow,
					"The DeltaEngine environment variable '" +
						BuilderViewModel.EnginePathEnvironmentVariableName + "' isn't set.\n" +
						"Please make sure it's defined correctly.",
					ShortName + " plugin");
			}
		}

		private void OnBrowseUserProjectClicked(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = CreateUserProjectPathBrowseDialog();
			if (dialog.ShowDialog().Equals(true))
				viewModel.UserSolutionPath = dialog.FileName;
		}

		private OpenFileDialog CreateUserProjectPathBrowseDialog()
		{
			return new OpenFileDialog
			{
				DefaultExt = ".sln",
				Filter = "C# Solution (.sln)|*.sln",
				InitialDirectory = GetInitialDirectoryForBrowseDialog(),
			};
		}

		protected virtual string GetInitialDirectoryForBrowseDialog()
		{
			return "";
		}

		public string ShortName
		{
			get { return "Builder"; }
		}

		public string Icon
		{
			get { return @"Icons/Builder.png"; }
		}

		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Builder; }
		}

		public int Priority
		{
			get { return 2; }
		}
	}
}