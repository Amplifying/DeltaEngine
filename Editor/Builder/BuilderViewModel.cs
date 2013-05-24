using System;
using System.Collections.Generic;
using System.IO;
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
			// TODO:
			this.service = service;
			//this.service = new BuildServiceConnectionViaLAN();
			service.MessageReceived += OnServiceMessageReceived;
			MessagesListViewModel = new BuildMessagesListViewModel();
			UserProjectEntryPoints = new List<string>();
			UserProjectEntryPoints.Add(DefaultEntryPoint);
			UserSelectedEntryPoint = DefaultEntryPoint;
			BuildPressed = new RelayCommand(OnBuildExecuted, CanBuildExecuted);
		}

		protected readonly Service service;
		public BuildMessagesListViewModel MessagesListViewModel { get; set; }

		public PlatformName[] SupportedPlatforms
		{
			// TODO: Doesn't contain all available platforms
			//get { return service.AllowedPlatforms; }
			get { return new[] { PlatformName.WindowsPhone7, PlatformName.Android, }; }
		}

		public List<string> UserProjectEntryPoints { get; set; }
		private const string DefaultEntryPoint = "Program.Main";
		public string UserSelectedEntryPoint { get; set; }
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
			string projectName = Path.GetFileNameWithoutExtension(UserProjectPath);
			var projectData = new CodeData(Path.GetDirectoryName(UserProjectPath));
			var request = new BuildRequest(projectName, SelectedPlatform, projectData.GetBytes())
			{
				SolutionFilePath = Path.GetFileName(userProjectPath),
			};
			service.SendMessage(request);
		}

		private BuildMessage GetErrorMessage(Exception ex)
		{
			string errorMessage = "Failed to send BuildRequest to server because " + ex.Message;
			return new BuildMessage(errorMessage)
			{
				Filename = Path.GetFileName(UserProjectPath),
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
			return File.Exists(UserProjectPath);
		}

		public string UserProjectPath
		{
			get { return userProjectPath; }
			set
			{
				userProjectPath = value;
				RaisePropertyChanged("UserProjectPath");
			}
		}
		private string userProjectPath;

		private bool IsUserSelectedEntryPointValid()
		{
			return UserSelectedEntryPoint == DefaultEntryPoint;
		}

		public PlatformName UserSelectedPlatform { get; set; }

		public void OnBrowseUserProjectExecuted(string newUserProjectPath)
		{
			UserProjectPath = newUserProjectPath;
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