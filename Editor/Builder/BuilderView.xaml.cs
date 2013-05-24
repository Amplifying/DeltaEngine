using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace DeltaEngine.Editor.Builder
{
	/// <summary>
	/// Interaction logic for LauncherView.xaml
	/// </summary>
	public partial class BuilderView : UserControl
	{
		public BuilderView(BuilderViewModel viewModel)
		{
			InitializeComponent();
			this.viewModel = viewModel;
			DataContext = viewModel;
			BuildMessagesList.MessagesViewModel = viewModel.MessagesListViewModel;
		}

		private readonly BuilderViewModel viewModel;

		private void OnBrowseUserProjectClicked(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = CreateUserProjectPathBrowseDialog();
			if (dialog.ShowDialog().Equals(true))
				viewModel.UserProjectPath = dialog.FileName;
		}

		private OpenFileDialog CreateUserProjectPathBrowseDialog()
		{
			return new OpenFileDialog
			{
				DefaultExt = ".sln",
				Filter = "C# Project Solution (.sln)|*.sln",
				InitialDirectory = GetInitialDirectoryForBrowseDialog(),
			};
		}

		protected virtual string GetInitialDirectoryForBrowseDialog()
		{
			return "";
		}
	}
}