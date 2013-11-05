using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// ViewModel of the the whole AppBuilder which manages all available actions like building an app
	/// for a specific platform, listing of all possbile build issues of the app and also provides a
	/// listing of already built apps in past.
	/// </summary>
	public class AppBuilderViewModel : ViewModelBase
	{
		public AppBuilderViewModel(Service service)
		{
			Service = service;
			MessagesListViewModel = new AppBuildMessagesListViewModel();
			AppListViewModel = new BuiltAppsListViewModel(service.EditorSettings);
			AppListViewModel.RebuildRequest += OnAppRebuildRequest;
			BuildCommand = new RelayCommand(OnBuildExecuted, () => IsBuildActionExecutable);
			HelpCommand = new RelayCommand(OpenHelpButtonClicked);
			service.ProjectChanged += OnContentProjectChanged;
			OnContentProjectChanged();
			service.DataReceived += OnServiceMessageReceived;
			Service.Send(new SupportedPlatformsRequest());
			SelectedPlatform = PlatformName.Windows;
		}

		public PlatformName[] SupportedPlatforms { get; private set; }
		public Service Service { get; private set; }
		public AppBuildMessagesListViewModel MessagesListViewModel { get; set; }
		public BuiltAppsListViewModel AppListViewModel { get; set; }
		public ICommand BuildCommand { get; private set; }
		public ICommand HelpCommand { get; private set; }

		private void OnAppRebuildRequest(AppInfo app)
		{
			TrySendBuildRequestToServer(app.SolutionFilePath, app.Name, app.Platform);
		}

		private void TrySendBuildRequestToServer(string solutionFilePath,
			string projectNameInSolution, PlatformName platform)
		{
			try
			{
				codeSolutionPathOfBuildingApp = UserSolutionPath;
				RaisePropertyChangedForIsBuildActionExecutable();
				SendBuildRequestToServer(solutionFilePath, projectNameInSolution, platform);
			}
			// ncrunch: no coverage start
			catch (Exception ex)
			{
				OnAppBuildMessageRecieved(GetErrorMessage(ex));
			}
			// ncrunch: no coverage end
		}

		private void RaisePropertyChangedForIsBuildActionExecutable()
		{
			RaisePropertyChanged("IsBuildActionExecutable");
		}

		private void SendBuildRequestToServer(string solutionFilePath, string projectNameInSolution,
			PlatformName platform)
		{
			var projectData = new CodePacker(solutionFilePath, projectNameInSolution);
			var request = new AppBuildRequest(Path.GetFileName(solutionFilePath), projectNameInSolution,
				platform, projectData.GetPackedData());
			Service.Send(request);
		}

		public string UserSolutionPath
		{
			get { return userSolutionPath; }
			set
			{
				userSolutionPath = value;
				RaisePropertyChanged("UserSolutionPath");
				DetermineAvailableProjectsInSpecifiedSolution();
				SelectedSolutionProject = FindUserProjectInSolution();
			}
		}

		private string userSolutionPath;
		private string codeSolutionPathOfBuildingApp;

		private void DetermineAvailableProjectsInSpecifiedSolution()
		{
			var solutionLoader = new SolutionFileLoader(UserSolutionPath);
			availableProjectsInSelectedSolution = solutionLoader.GetCSharpProjects();
		}

		private List<ProjectEntry> availableProjectsInSelectedSolution;

		private ProjectEntry FindUserProjectInSolution()
		{
			return availableProjectsInSelectedSolution.FirstOrDefault(
				csProject => csProject.Title == Service.ProjectName);
		}

		public ProjectEntry SelectedSolutionProject
		{
			get { return selectedSolutionProject; }
			set
			{
				selectedSolutionProject = value;
				RaisePropertyChanged("SelectedSolutionProject");
				RaisePropertyChangedForIsBuildActionExecutable();
				DetermineEntryPointsOfProject();
			}
		}

		private ProjectEntry selectedSolutionProject;

		private void DetermineEntryPointsOfProject()
		{
			AvailableEntryPointsInSelectedProject = new List<string>();
			AvailableEntryPointsInSelectedProject.Add(DefaultEntryPoint);
			RaisePropertyChanged("AvailableEntryPointsInSelectedProject");
			SelectedEntryPoint = DefaultEntryPoint;
			RaisePropertyChanged("SelectedEntryPoint");
		}

		public List<string> AvailableEntryPointsInSelectedProject { get; set; }
		private const string DefaultEntryPoint = "Program.Main";

		public string SelectedEntryPoint
		{
			get { return selectedEntryPoint; }
			set
			{
				selectedEntryPoint = value;
				RaisePropertyChangedForIsBuildActionExecutable();
			}
		}

		private string selectedEntryPoint;

		public PlatformName SelectedPlatform
		{
			get { return selectedPlatform; }
			set
			{
				selectedPlatform = value;
				RaisePropertyChanged("SelectedPlatform");
				RaisePropertyChangedForIsBuildActionExecutable();
			}
		}

		private PlatformName selectedPlatform;

		// ncrunch: no coverage start
		private AppBuildMessage GetErrorMessage(Exception ex)
		{
			string errorMessage = "Failed to send BuildRequest to server because " + ex;
			return new AppBuildMessage(errorMessage)
			{
				Filename = Path.GetFileName(UserSolutionPath),
				Type = AppBuildMessageType.BuildError,
			};
		}
		// ncrunch: no coverage end

		protected void OnBuildExecuted()
		{
			Logger.Info("Build Request sent");
			MessagesListViewModel.ClearMessages();
			TrySendBuildRequestToServer(UserSolutionPath, SelectedSolutionProject.Title,
				SelectedPlatform);
		}

		// ncrunch: no coverage start
		private static void OpenHelpButtonClicked()
		{
			Process.Start("http://DeltaEngine.net/features/appbuilder");
		}
		// ncrunch: no coverage end

		private void OnContentProjectChanged()
		{
			TrySelectEngineSamplesSolution();
		}

		private void OnServiceMessageReceived(object serviceMessage)
		{
			if (serviceMessage is SupportedPlatformsResult)
				OnSupportedPlatformsResultRecieved((SupportedPlatformsResult)serviceMessage);
			if (serviceMessage is AppBuildMessage)
				OnAppBuildMessageRecieved((AppBuildMessage)serviceMessage);
			if (serviceMessage is AppBuildResult)
				OnAppBuildResultRecieved((AppBuildResult)serviceMessage);
			if (serviceMessage is AppBuildFailed)
				OnAppBuildFailedRecieved((AppBuildFailed)serviceMessage);
		}

		private void OnSupportedPlatformsResultRecieved(SupportedPlatformsResult platformsMessage)
		{
			SupportedPlatforms = platformsMessage.Platforms;
			RaisePropertyChanged("SupportedPlatforms");
			RaisePropertyChangedForIsBuildActionExecutable();
		}

		private void OnAppBuildMessageRecieved(AppBuildMessage receivedMessage)
		{
			if (receivedMessage.Type == AppBuildMessageType.BuildInfo)
			{
				Logger.Info(receivedMessage.Text);
				return;
			}
			Logger.Warning(receivedMessage.Text);
			MessagesListViewModel.AddMessage(receivedMessage);
			if (receivedMessage.Type == AppBuildMessageType.BuildError)
				AllowBuildingAppsAgain();
		}

		private void AllowBuildingAppsAgain()
		{
			codeSolutionPathOfBuildingApp = null;
			RaisePropertyChangedForIsBuildActionExecutable();
		}

		private void OnAppBuildResultRecieved(AppBuildResult receivedBuildResult)
		{
			AppInfo appInfo = AppInfoExtensions.CreateAppInfo(
				Path.Combine(AppListViewModel.AppStorageDirectory, receivedBuildResult.PackageFileName),
				receivedBuildResult.Platform, receivedBuildResult.PackageGuid, DateTime.Now);
			appInfo.SolutionFilePath = codeSolutionPathOfBuildingApp;
			AllowBuildingAppsAgain();
			TriggerBuiltAppRecieved(appInfo, receivedBuildResult.PackageFileData);
		}

		private void TriggerBuiltAppRecieved(AppInfo appInfo, byte[] appData)
		{
			if (BuiltAppRecieved != null)
				BuiltAppRecieved(appInfo, appData);
		}

		public event Action<AppInfo, byte[]> BuiltAppRecieved;

		private void OnAppBuildFailedRecieved(AppBuildFailed buildFailedMessage)
		{
			AllowBuildingAppsAgain();
			TriggerAppBuildFailedRecieved(buildFailedMessage);
		}

		private void TriggerAppBuildFailedRecieved(AppBuildFailed buildFailedMessage)
		{
			if (AppBuildFailedRecieved != null)
				AppBuildFailedRecieved(buildFailedMessage);
		}

		public event Action<AppBuildFailed> AppBuildFailedRecieved;

		private void TrySelectEngineSamplesSolution()
		{
			UserSolutionPath = GetSamplesSolutionFilePath();
			if (UserSolutionPath == null)
				LogSamplesSolutionNotFoundWarning(); // ncrunch: no coverage
		}

		public static string GetSamplesSolutionFilePath()
		{
			const string SamplesSolutionFile = "DeltaEngine.Samples.sln";
			return GetFilePathFromSourceCode(SamplesSolutionFile) ??
				GetFilePathFromInstallerRelease(SamplesSolutionFile);
		}

		private static string GetFilePathFromSourceCode(string filename)
		{
			string originalDirectory = Environment.CurrentDirectory;
			try
			{
				if (StackTraceExtensions.StartedFromNCrunch)
					Environment.CurrentDirectory = PathExtensions.GetFallbackEngineSourceCodeDirectory();
				var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
				return GetFilePathFromSourceCodeRecursively(currentDirectory, filename);
			}
			finally
			{
				Environment.CurrentDirectory = originalDirectory;
			}
		}

		private static string GetFilePathFromSourceCodeRecursively(DirectoryInfo directory,
			string filename)
		{
			if (directory.Parent == null)
				return null; //ncrunch: no coverage
			foreach (var file in directory.GetFiles())
				if (file.Name == filename)
					return file.FullName;
			return GetFilePathFromSourceCodeRecursively(directory.Parent, filename);
		}

		private static string GetFilePathFromInstallerRelease(string filePath)
		{
			return PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable()
				? Path.Combine(PathExtensions.GetDeltaEngineInstalledDirectory(),
				Path.Combine("OpenTK", filePath)) : null;
		}

		// ncrunch: no coverage start
		private static void LogSamplesSolutionNotFoundWarning()
		{
			string newLine = Environment.NewLine;
			Logger.Warning("AppBuilder plugin: The DeltaEngine.Samples.sln can't be find." + newLine +
				"Please make sure that the '" + PathExtensions.EnginePathEnvironmentVariableName +
				"' environment variable isn't set or alternatively you downloaded the Engine source code" +
				" to '" + PathExtensions.DefaultCodePath + ".");
		}
		// ncrunch: no coverage end

		public bool IsBuildActionExecutable
		{
			get
			{
				return IsCodeOfSelectedProjectAvailable && IsUserSelectedEntryPointValid &&
					IsSelectedPlatformValid && codeSolutionPathOfBuildingApp == null;
			}
		}

		private bool IsCodeOfSelectedProjectAvailable
		{
			get { return SelectedSolutionProject != null; }
		}

		private bool IsUserSelectedEntryPointValid
		{
			get { return SelectedEntryPoint == DefaultEntryPoint; }
		}

		private bool IsSelectedPlatformValid
		{
			get
			{
				return SupportedPlatforms != null &&
					SupportedPlatforms.Any(supportedPlatform => SelectedPlatform == supportedPlatform);
			}
		}
	}
}