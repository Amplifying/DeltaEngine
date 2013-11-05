using System;
using System.IO;
using System.Windows;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using GalaSoft.MvvmLight.Messaging;
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
			ViewModel.AppBuildFailedRecieved += DispatchAndHandleBuildFailedRecievedEvent;
			ViewModel.BuiltAppRecieved += DispatchAndHandleBuildAppReceivedEvent;
			BuildList.MessagesViewModel = ViewModel.MessagesListViewModel;
			BuildList.AppListViewModel = ViewModel.AppListViewModel;
			SwitchToBuiltApps();
			DataContext = ViewModel;
			Messenger.Default.Send("AppBuilder", "SetSelectedEditorPlugin");
		}

		public void Activate()
		{
			Messenger.Default.Send("AppBuilder", "SetSelectedEditorPlugin");
		}

		public AppBuilderViewModel ViewModel { get; private set; }

		private void DispatchAndHandleBuildFailedRecievedEvent(AppBuildFailed buildFailedMessage)
		{
			Dispatcher.BeginInvoke(new Action(() => OnAppBuildFailedRecieved(buildFailedMessage)));
		}

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

		private void SwitchToBuildMessagesList()
		{
			if (BuildList.BuiltAppsList.IsVisible)
				Dispatcher.BeginInvoke(new Action(BuildList.FocusBuildMessagesList));
		}

		private void DispatchAndHandleBuildAppReceivedEvent(AppInfo appInfo, byte[] appData)
		{
			Dispatcher.BeginInvoke(new Action(() => OnBuiltAppRecieved(appInfo, appData)));
		}

		private void OnBuiltAppRecieved(AppInfo appInfo, byte[] appData)
		{
			ViewModel.AppListViewModel.AddApp(appInfo, appData);
			SwitchToBuiltApps();
			InstallAndLaunchNewBuiltApp(appInfo);
		}

		private void SwitchToBuiltApps()
		{
			if (BuildList.BuildMessagesList.IsVisible)
				Dispatcher.BeginInvoke(new Action(BuildList.FocusBuiltAppsList));
		}

		private void InstallAndLaunchNewBuiltApp(AppInfo appInfo)
		{
			if (!appInfo.IsDeviceAvailable)
				throw new NoDeviceAvailable(appInfo);

			Device primaryDevice = appInfo.AvailableDevices[0];
			if (primaryDevice.IsAppInstalled(appInfo))
			{
				Logger.Info("App " + Name + " was already installed, uninstalling it.");
				primaryDevice.Uninstall(appInfo);
			}
			Logger.Info("Installing App " + Name + " on " + primaryDevice.Name);
			primaryDevice.Install(appInfo);
			primaryDevice.Launch(appInfo);
		}

		public class NoDeviceAvailable : Exception
		{
			public NoDeviceAvailable(AppInfo appInfo)
				: base(appInfo.ToString()) { }
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