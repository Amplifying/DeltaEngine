using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
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
			AppListViewModel = new BuiltAppsListViewModel();
			AppListViewModel.RebuildRequest += OnAppRebuildRequest;
			BuildPressed = new RelayCommand(OnBuildExecuted);
			BrowsePressed = new RelayCommand<string>(OnBrowseUserSolutionPathExecuted);
			service.DataReceived += OnServiceMessageReceived;
			RequestSupportedPlatforms();
		}

		public PlatformName[] SupportedPlatforms { get; private set; }

		public Service Service { get; private set; }
		public AppBuildMessagesListViewModel MessagesListViewModel { get; set; }
		public BuiltAppsListViewModel AppListViewModel { get; set; }
		public ICommand BuildPressed { get; private set; }
		public ICommand BrowsePressed { get; private set; }

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
			catch (Exception ex)
			{
				OnAppBuildMessageRecieved(GetErrorMessage(ex));
			}
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
				DetermineAvailableProjectsOfSamplesSolution();
			}
		}
		private string userSolutionPath;
		private string codeSolutionPathOfBuildingApp;

		private void DetermineAvailableProjectsOfSamplesSolution()
		{
			AvailableProjectsInSelectedSolution = new List<ProjectEntry>();
			var solutionLoader = new SolutionFileLoader(UserSolutionPath);
			List<ProjectEntry> allProjects = solutionLoader.GetCSharpProjects();
			foreach (var project in allProjects.Where(project => IsSampleProject(project)))
				AvailableProjectsInSelectedSolution.Add(project);
			RaisePropertyChanged("AvailableProjectsInSelectedSolution");
			SelectedSolutionProject = FindUserProjectInSolution();
		}

		public List<ProjectEntry> AvailableProjectsInSelectedSolution { get; set; }

		private static bool IsSampleProject(ProjectEntry project)
		{
			return !project.Title.EndsWith(".Tests") && !project.Title.StartsWith("DeltaEngine.") &&
				!project.Title.StartsWith("Empty");
		}

		private ProjectEntry FindUserProjectInSolution()
		{
			return
				AvailableProjectsInSelectedSolution.FirstOrDefault(
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
				RaisePropertyChangedForIsBuildActionExecutable();
			}
		}
		private PlatformName selectedPlatform;

		private AppBuildMessage GetErrorMessage(Exception ex)
		{
			string errorMessage = "Failed to send BuildRequest to server because " + ex.Message;
			return new AppBuildMessage(errorMessage)
			{
				Filename = Path.GetFileName(UserSolutionPath),
				Type = AppBuildMessageType.BuildError,
			};
		}

		protected void OnBuildExecuted()
		{
			TrySendBuildRequestToServer(UserSolutionPath, SelectedSolutionProject.Title,
				SelectedPlatform);
		}

		private void OnBrowseUserSolutionPathExecuted(string newUserProjectPath)
		{
			UserSolutionPath = newUserProjectPath;
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
		}

		private void OnAppBuildMessageRecieved(AppBuildMessage receivedMessage)
		{
			if (receivedMessage.Type == AppBuildMessageType.BuildInfo)
				return;
			MessagesListViewModel.AddMessage(receivedMessage);
			AllowBuildingAppsAgain();
		}

		private void AllowBuildingAppsAgain()
		{
			codeSolutionPathOfBuildingApp = null;
			RaisePropertyChangedForIsBuildActionExecutable();
		}

		private void OnAppBuildResultRecieved(AppBuildResult receivedBuildResult)
		{
			AppInfo appInfo = receivedBuildResult.ToAppInfo(AppListViewModel.AppStorageDirectory);
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

		private void RequestSupportedPlatforms()
		{
			SupportedPlatforms = new[] { PlatformName.Windows, };
			SelectedPlatform = PlatformName.Windows;
			Service.Send(new SupportedPlatformsRequest());
		}

		public bool IsBuildActionExecutable
		{
			get
			{
				return IsUserProjectPathValid && IsUserSelectedEntryPointValid && IsSelectedPlatformValid &&
					codeSolutionPathOfBuildingApp == null;
			}
		}

		private bool IsUserProjectPathValid
		{
			get { return File.Exists(UserSolutionPath); }
		}

		private bool IsUserSelectedEntryPointValid
		{
			get { return SelectedEntryPoint == DefaultEntryPoint; }
		}

		private bool IsSelectedPlatformValid
		{
			get { return SupportedPlatforms.Any(supportedPlatform => SelectedPlatform == supportedPlatform); }
		}
	}
}