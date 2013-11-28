using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Networking.Messages;
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
			availableCodeProjectsWithContentProjects = new Dictionary<ProjectEntry, string>();
			MessagesListViewModel = new AppBuildMessagesListViewModel();
			AppListViewModel = new BuiltAppsListViewModel(Settings.Current);
			AppListViewModel.RebuildRequest += OnAppRebuildRequest;
			BuildCommand = new RelayCommand(OnBuildExecuted, () => IsBuildActionExecutable);
			HelpCommand = new RelayCommand(OpenAppBuilderFeaturesPage);
			GotoUserProfilePageCommand = new RelayCommand(OpenUserProfilePage);
			GotoBuiltAppsDirectoryCommand = new RelayCommand(OpenLocalBuiltAppsDirectory);
			service.AvailableProjectsChanged += OnAvailableProjectsChanged;
			OnAvailableProjectsChanged();
			service.ProjectChanged += OnContentProjectChanged;
			OnContentProjectChanged();
			service.DataReceived += OnServiceMessageReceived;
			Service.Send(new SupportedPlatformsRequest());
			SelectedPlatform = PlatformName.Windows;
			IsRebuildForced = false;
		}

		private readonly Dictionary<ProjectEntry, string> availableCodeProjectsWithContentProjects;
		public PlatformName[] SupportedPlatforms { get; private set; }
		public Service Service { get; private set; }
		public AppBuildMessagesListViewModel MessagesListViewModel { get; set; }
		public BuiltAppsListViewModel AppListViewModel { get; set; }
		public ICommand BuildCommand { get; private set; }
		public ICommand HelpCommand { get; private set; }
		public ICommand GotoUserProfilePageCommand { get; private set; }
		public ICommand GotoBuiltAppsDirectoryCommand { get; private set; }

		private void OnAppRebuildRequest(AppInfo app)
		{
			TrySendBuildRequestToServer(app.SolutionFilePath, app.Name, app.Platform, true);
		}

		private void TrySendBuildRequestToServer(string solutionFilePath,
			string projectNameInSolution, PlatformName platform, bool isRebuildOfCodeForced)
		{
			try
			{
				codeSolutionPathOfBuildingApp = UserSolutionPath;
				RaisePropertyChangedForIsBuildActionExecutable();
				SendBuildRequestToServer(solutionFilePath, projectNameInSolution, platform,
					isRebuildOfCodeForced);
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
			PlatformName platform, bool isRebuildOfCodeForced)
		{
			var projectData = new CodePacker(solutionFilePath, projectNameInSolution);
			var request = new AppBuildRequest(Path.GetFileName(solutionFilePath), projectNameInSolution,
				platform, projectData.GetPackedData());
			request.IsRebuildOfCodeForced = isRebuildOfCodeForced;
			request.ContentProjectName = GetContentProject(projectNameInSolution);
			Service.Send(request, false);
		}

		private string GetContentProject(string codeProjectName)
		{
			foreach (var codeProjectAndContentProject in availableCodeProjectsWithContentProjects)
				if (codeProjectAndContentProject.Key.Name == codeProjectName)
					return codeProjectAndContentProject.Value;
			throw new NoContentProjectAvailableForCodeProject(codeProjectName);
		}

		public class NoContentProjectAvailableForCodeProject : Exception
		{
			public NoContentProjectAvailableForCodeProject(string codeProjectName)
				: base(codeProjectName) {}
		}

		public string UserSolutionPath
		{
			get { return userSolutionPath; }
			set
			{
				userSolutionPath = value;
				RaisePropertyChanged("UserSolutionPath");
				RaisePropertyChangedForIsBuildActionExecutable();
			}
		}

		private string userSolutionPath;

		private string codeSolutionPathOfBuildingApp;

		private void OnAvailableProjectsChanged()
		{
			List<string> solutionList = GetFilePathListOfAvailableCodeSolutions();
			availableCodeProjectsWithContentProjects.Clear();
			foreach (string solutionFilePath in solutionList)
				AddCodeProjectsOfSolutionToAvailableProjects(solutionFilePath);
			RaisePropertyChanged("AllAvailableProjects");
			AutoSelectNewCodeProject();
		}

		private void AutoSelectNewCodeProject()
		{
			SelectedCodeProject = FindUserProjectInSolution();
			if (SelectedCodeProject == null)
				return;
		}

		private List<string> GetFilePathListOfAvailableCodeSolutions()
		{
			var solutionList = new List<string>();
			foreach (string contentProject in Service.AvailableProjects)
			{
				string solutionFilePath = Service.GetAbsoluteSolutionFilePath(contentProject);
				if (String.IsNullOrEmpty(solutionFilePath) || solutionList.Contains(solutionFilePath))
					continue;
				solutionList.Add(solutionFilePath);
				if (contentProject == "DeltaEngine.Tutorials")
					solutionList.Add(solutionFilePath.Replace("Basics", "Entities"));
			}
			return solutionList;
		}

		private void AddCodeProjectsOfSolutionToAvailableProjects(string solutionFilePath)
		{
			var solutionLoader = new SolutionFileLoader(solutionFilePath);
			var availableProjectsInSolution = solutionLoader.GetCSharpProjects();
			foreach (ProjectEntry projectEntry in availableProjectsInSolution)
			{
				string contentProject = GetContentProject(projectEntry);
				if (!String.IsNullOrEmpty(contentProject))
					if (!IsProjectAlreadyAddedToDictionary(projectEntry))
						availableCodeProjectsWithContentProjects.Add(projectEntry, contentProject);
			}
		}

		private string GetContentProject(ProjectEntry project)
		{
			foreach (string contentProject in Service.AvailableProjects)
				if (!project.Name.EndsWith(".Tests") && project.Name.StartsWith(contentProject))
					return contentProject;
			return "";
		}

		private bool IsProjectAlreadyAddedToDictionary(ProjectEntry projectEntry)
		{
			foreach (ProjectEntry codeProject in availableCodeProjectsWithContentProjects.Keys)
				if (codeProject.Name == projectEntry.Name)
					return true;
			return false;
		}

		public ICollection<ProjectEntry> AllAvailableProjects
		{
			get { return availableCodeProjectsWithContentProjects.Keys; }
		}

		private ProjectEntry FindUserProjectInSolution()
		{
			return AllAvailableProjects.FirstOrDefault(
				csProject => csProject.Name.StartsWith(Service.ProjectName));
		}

		public ProjectEntry SelectedCodeProject
		{
			get { return selectedCodeProject; }
			set
			{
				// Temporarly HACK for the Presentation: For some reason the this triggered a second time
				// with just "null"
				if (value == null && IsSelectedProjectStillSelectable())
					return;
				selectedCodeProject = value;
				RaisePropertyChanged("SelectedCodeProject");
				string contentProjectName;
				if (value != null)
					if (availableCodeProjectsWithContentProjects.TryGetValue(value, out contentProjectName))
						UpdateUserSolutionPath(contentProjectName);
				DetermineEntryPointsOfProject();
				RaisePropertyChangedForIsBuildActionExecutable();
			}
		}

		private bool IsSelectedProjectStillSelectable()
		{
			return selectedCodeProject != null &&
				AllAvailableProjects.Contains(selectedCodeProject);
		}

		private ProjectEntry selectedCodeProject;

		private void UpdateUserSolutionPath(string contentProjectName)
		{
			UserSolutionPath = Service.GetAbsoluteSolutionFilePath(contentProjectName);
		}

		private void DetermineEntryPointsOfProject()
		{
			AvailableEntryPointsInSelectedProject = new List<string>();
			if (SelectedCodeProject == null)
				return;
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
			TrySendBuildRequestToServer(UserSolutionPath, SelectedCodeProject.Name, SelectedPlatform,
				IsRebuildForced);
		}

		// ncrunch: no coverage start
		private static void OpenAppBuilderFeaturesPage()
		{
			Process.Start("http://http://deltaengine.net/features/appbuilder");
		}

		private static void OpenUserProfilePage()
		{
			Process.Start("http://deltaengine.net/account/Projects/");
		}

		private void OpenLocalBuiltAppsDirectory()
		{
			Process.Start(AppListViewModel.AppStorageDirectory);
		}
		// ncrunch: no coverage end

		private void OnContentProjectChanged()
		{
			UpdateUserSolutionPath(Service.ProjectName);
			AutoSelectNewCodeProject();
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
			if (serviceMessage is ServerError)
				OnServerError((ServerError)serviceMessage);
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

		private void OnServerError(ServerError serverError)
		{
			OnAppBuildFailedRecieved(new AppBuildFailed(serverError.Error));
		}

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
			get { return SelectedCodeProject != null; }
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

		public bool IsRebuildForced { get; set; }

		public void OverrideUserSolutionPathWithCustomPath(string newUserSolutionPath)
		{
			UserSolutionPath = newUserSolutionPath;
			Service.CurrentContentProjectSolutionFilePath = newUserSolutionPath;
			OnAvailableProjectsChanged();
		}
	}
}