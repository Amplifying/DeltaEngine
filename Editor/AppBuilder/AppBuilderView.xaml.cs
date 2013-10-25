using System;
using System.IO;
using System.Windows;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using Microsoft.Win32;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Shows all available actions and data provided by the AppBuilderViewModel.
	/// </summary>
	public partial class AppBuilderView : EditorPluginView
	{
		public AppBuilderView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			ViewModel = new AppBuilderViewModel(service);
			ViewModel.AppBuildFailedRecieved += OnAppBuildFailedRecieved;
			ViewModel.BuiltAppRecieved += OnBuiltAppRecieved;
			BuildList.MessagesViewModel = ViewModel.MessagesListViewModel;
			BuildList.AppListViewModel = ViewModel.AppListViewModel;
			SwitchToBuiltApps();
			DataContext = ViewModel;
		}

		public void ProjectChanged() {}

		public AppBuilderViewModel ViewModel { get; private set; }

		private void OnAppBuildFailedRecieved(AppBuildFailed buildFailedMessage)
		{
			var errorMessage = new AppBuildMessage(buildFailedMessage.Reason)
			{
				Project = ViewModel.Service.ProjectName,
				Type = AppBuildMessageType.BuildError,
			};
			ViewModel.MessagesListViewModel.AddMessage(errorMessage);
			SwitchToBuildMessagesList();
		}

		private void OnBuiltAppRecieved(AppInfo appInfo, byte[] appData)
		{
			ViewModel.AppListViewModel.AddApp(appInfo, appData);
			SwitchToBuiltApps();
		}

		private void SwitchToBuiltApps()
		{
			if (BuildList.BuildMessagesList.IsVisible)
				Dispatcher.BeginInvoke(new Action(BuildList.FocusBuiltAppsList));
		}

		private void SwitchToBuildMessagesList()
		{
			if (BuildList.BuiltAppsList.IsVisible)
				Dispatcher.BeginInvoke(new Action(BuildList.FocusBuildMessagesList));
		}

		private void OnBrowseUserProjectClicked(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = CreateUserProjectPathBrowseDialog();
			if (dialog.ShowDialog().Equals(true))
				ViewModel.UserSolutionPath = dialog.FileName;
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

		private string GetInitialDirectoryForBrowseDialog()
		{
			if (String.IsNullOrWhiteSpace(ViewModel.UserSolutionPath))
				return PathExtensions.GetDeltaEngineInstalledDirectory();
			return Path.GetDirectoryName(ViewModel.UserSolutionPath);
		}

		public string ShortName
		{
			get { return "App Builder"; }
		}

		public string Icon
		{
			get { return @"Images/Plugins/AppBuilder.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}
	}
}