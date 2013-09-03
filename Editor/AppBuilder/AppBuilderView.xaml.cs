using System;
using System.IO;
using System.Windows;
using DeltaEngine.Core;
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
			TrySelectEngineSamplesSolution();
			DataContext = ViewModel;
		}

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

		private void TrySelectEngineSamplesSolution()
		{
			string engineDirectory = PathExtensions.GetDeltaEngineDirectory();
			if (engineDirectory != null)
				ViewModel.UserSolutionPath = Path.Combine(engineDirectory, "DeltaEngine.Samples.sln");
			else
				Logger.Warning(ShortName + " plugin: The DeltaEngine environment variable '" +
					PathExtensions.EnginePathEnvironmentVariableName + "' isn't set." + Environment.NewLine +
					"Please make sure it's defined correctly.");
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

		protected string GetInitialDirectoryForBrowseDialog()
		{
			return "";
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