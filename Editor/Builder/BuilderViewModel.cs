using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DeltaEngine.Editor.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.Builder
{
	public class BuilderViewModel : ViewModelBase
	{
		public BuilderViewModel(Service service)
		{
			this.Service = service;
			service.MessageReceived += OnServiceMessageReceived;
			MessagesListViewModel = new BuildMessagesListViewModel();
			AppListViewModel = new BuiltAppsListViewModel();
			BuildPressed = new RelayCommand(OnBuildExecuted, CanBuildExecuted);
			SelectFirstAvailablePlatform();
		}

		public Service Service { get; private set; }
		public BuildMessagesListViewModel MessagesListViewModel { get; set; }
		public BuiltAppsListViewModel AppListViewModel { get; set; }
		public ICommand BuildPressed { get; private set; }

		protected virtual void OnBuildExecuted()
		{
			try
			{
				SendBuildRequestToServer();
			}
			catch (Exception ex)
			{
				MessagesListViewModel.AddMessage(GetErrorMessage(ex));
			}
		}

		private void SendBuildRequestToServer()
		{
			string projectName = Path.GetFileNameWithoutExtension(UserSolutionPath);
			var projectData = new CodeData(Path.GetDirectoryName(UserSolutionPath));
			var request = new BuildRequest(projectName, SelectedPlatform, projectData.GetBytes())
			{
				SolutionFilePath = Path.GetFileName(userSolutionPath),
			};
			Service.SendMessage(request);
		}

		private BuildMessage GetErrorMessage(Exception ex)
		{
			string errorMessage = "Failed to send BuildRequest to server because " + ex.Message;
			return new BuildMessage(errorMessage)
			{
				Filename = Path.GetFileName(UserSolutionPath),
				Type = BuildMessageType.BuildError,
			};
		}

		public PlatformName SelectedPlatform { get; set; }

		protected virtual bool CanBuildExecuted()
		{
			return IsUserProjectPathValid() && IsUserSelectedEntryPointValid();
		}

		private bool IsUserProjectPathValid()
		{
			return File.Exists(UserSolutionPath);
		}

		private bool IsUserSelectedEntryPointValid()
		{
			return SelectedEntryPoint == DefaultEntryPoint;
		}

		private void SelectFirstAvailablePlatform()
		{
			UserSelectedPlatform = Service.AllowedPlatforms[0];
		}

		public PlatformName UserSelectedPlatform { get; set; }

		public void SelectSamplesSolution()
		{
			string engineDirectory = Environment.GetEnvironmentVariable(EnginePathEnvironmentVariableName);
			if (engineDirectory == null)
				return; //throw new DeltaEnginePathUnknown();

			UserSolutionPath = Path.Combine(engineDirectory, "DeltaEngine.Samples.sln");
		}

		public const string EnginePathEnvironmentVariableName = "DeltaEnginePath";

		public class DeltaEnginePathUnknown : Exception {}

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

		private void DetermineAvailableProjectsOfSamplesSolution()
		{
			AvailableProjectsInSelectedSolution = new List<ProjectEntry>();
			var solutionLoader = new SolutionFileLoader(UserSolutionPath);
			List<ProjectEntry> allProjects = solutionLoader.GetCSharpProjects();
			foreach (var project in allProjects.Where(project => IsSampleProject(project)))
				AvailableProjectsInSelectedSolution.Add(project);
			RaisePropertyChanged("AvailableProjectsInSelectedSolution");
			SelectedProject = AvailableProjectsInSelectedSolution[0];
		}

		public List<ProjectEntry> AvailableProjectsInSelectedSolution { get; set; }

		private static bool IsSampleProject(ProjectEntry project)
		{
			return !project.Title.EndsWith(".Tests") && !project.Title.StartsWith("DeltaEngine.") &&
				!project.Title.StartsWith("Empty");
		}

		public ProjectEntry SelectedProject
		{
			get { return selectedProject; }
			set
			{
				selectedProject = value;
				RaisePropertyChanged("SelectedProject");
				DetermineEntryPointsOfProject();
			}
		}
		private ProjectEntry selectedProject;

		private void DetermineEntryPointsOfProject()
		{
			AvailableEntryPointsInSelectedProject = new List<string>();
			AvailableEntryPointsInSelectedProject.Add(DefaultEntryPoint);
			SelectedEntryPoint = DefaultEntryPoint;
		}

		public List<string> AvailableEntryPointsInSelectedProject { get; set; }
		private const string DefaultEntryPoint = "Program.Main";
		public string SelectedEntryPoint { get; set; }

		public PlatformName[] SupportedPlatforms
		{
			get { return Service.AllowedPlatforms; }
		}

		public void OnBrowseUserProjectExecuted(string newUserProjectPath)
		{
			UserSolutionPath = newUserProjectPath;
		}

		private void OnServiceMessageReceived(object serviceMessage)
		{
			if (serviceMessage is BuildMessage)
				OnBuildMessageRecieved((BuildMessage)serviceMessage);

			if (serviceMessage is BuildResult)
				OnBuildResultRecieved((BuildResult)serviceMessage);
		}

		private void OnBuildMessageRecieved(BuildMessage receivedMessage)
		{
			MessagesListViewModel.AddMessage(receivedMessage);
		}

		private void OnBuildResultRecieved(BuildResult receivedBuildResult)
		{
			File.WriteAllBytes(Path.Combine(@"C:\code\Packages", receivedBuildResult.PackageFileName),
				receivedBuildResult.PackageFileData);
			MessagesListViewModel.AddMessage(new BuildMessage("Build done."));
		}
	}
}