using System.Diagnostics;
using System.IO.Abstractions;
using System.Windows;
using System.Windows.Input;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.ProjectCreator
{
	public class ProjectCreatorViewModel : ViewModelBase
	{
		public ProjectCreatorViewModel(Service service)
		{
			this.service = service;
			this.service.DataReceived += OnDataReceived;
			project = new CsProject(service.UserName);
			AvailableStarterKits = VsTemplate.GetAllTemplateNames(project.Framework);
			AvailableFrameworks = new FrameworkFinder().All;
			RegisterCommands();
		}

		private readonly Service service;

		private void OnDataReceived(object message)
		{
			var createProject = message as CreateProject;
			if (createProject != null)
				OpenProject(createProject);
		}

		private void OpenProject(CreateProject createProject)
		{
			Logger.Info("Content Project " + createProject.ProjectName + " has been created");
			Process.Start(service.CurrentContentProjectSolutionFilePath);
		}

		private readonly CsProject project;
		public string[] AvailableStarterKits { get; private set; }
		public DeltaEngineFramework[] AvailableFrameworks { get; private set; }

		private void RegisterCommands()
		{
			OnNameChanged = new RelayCommand<string>(ChangeName);
			OnFrameworkSelectionChanged = new RelayCommand<int>(ChangeSelection);
			OnLocationChanged = new RelayCommand<string>(ChangeLocation);
			OnCreateClicked = new RelayCommand(CreateProject, CanCreateProject);
		}

		public ICommand OnNameChanged { get; private set; }

		private void ChangeName(string projectName)
		{
			Name = projectName;
		}

		public string Name
		{
			get { return project.Name; }
			set
			{
				project.Name = value;
				RaisePropertyChanged("Name");
			}
		}

		public ICommand OnFrameworkSelectionChanged { get; private set; }

		private void ChangeSelection(int index)
		{
			SelectedFramework = (DeltaEngineFramework)index;
		}

		public DeltaEngineFramework SelectedFramework
		{
			get { return project.Framework; }
			set
			{
				project.Framework = value;
				RaisePropertyChanged("SelectedFramework");
			}
		}

		public string SelectedStarterKit
		{
			get { return project.StarterKit; }
			set
			{
				project.StarterKit = value;
				RaisePropertyChanged("SelectedStarterKit");
				RaisePropertyChanged("Name");
			}
		}

		public ICommand OnLocationChanged { get; private set; }

		private void ChangeLocation(string projectPath)
		{
			Location = projectPath;
		}

		public string Location
		{
			get { return project.BaseDirectory; }
			set
			{
				project.BaseDirectory = value;
				RaisePropertyChanged("Location");
			}
		}

		public ICommand OnCreateClicked { get; private set; }

		private void CreateProject()
		{
			var projectCreator = new ProjectCreator(project,
				VsTemplate.CreateByName(new FileSystem(), project.StarterKit), service, new FileSystem());
			projectCreator.CreateProject();
			if (projectCreator.HaveTemplateFilesBeenCopiedToLocation())
				service.Send(new CreateProject(projectCreator.Project.Name, project.StarterKit));
			else
				MessageBox.Show(
					"Project has not been created. " +
						"Please make sure the specified location and the VisualStudioTemplates are available.",
					"Error");
		}

		private bool CanCreateProject()
		{
			return InputValidator.IsValidProjectName(Name) &&
				InputValidator.IsValidAbsolutePath(Location);
		}
	}
}