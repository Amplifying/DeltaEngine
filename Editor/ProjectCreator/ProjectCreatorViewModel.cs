using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Windows;
using System.Windows.Input;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.ProjectCreator
{
	public class ProjectCreatorViewModel : ViewModelBase
	{
		public ProjectCreatorViewModel()
		{
			frameworks = new FrameworkFinder();
			project = new CsProject();
			AvailableFrameworks = frameworks.All;
			RegisterCommands();
		}

		private readonly FrameworkFinder frameworks;
		private readonly CsProject project;
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

		public ICommand OnLocationChanged { get; private set; }

		private void ChangeLocation(string projectPath)
		{
			Location = projectPath;
		}

		public string Location
		{
			get { return project.Location; }
			set
			{
				project.Location = value;
				RaisePropertyChanged("Location");
			}
		}

		public ICommand OnCreateClicked { get; private set; }

		private void CreateProject()
		{
			var projectCreator = new ProjectCreator(project, VsTemplate.GetEmptyGame(new FileSystem()),
				new FileSystem());
			projectCreator.CreateProject();
			if (projectCreator.HaveTemplateFilesBeenCopiedToLocation())
			{
				Service.Send(new CreateProject(projectCreator.Project.Name));
				MessageBox.Show("Project has successfully been created.", "Project created");
			}
			else
				MessageBox.Show(
					"Project has not been created. " +
						"Please make sure the specified location and the VisualStudioTemplates are available.",
					"Error");
		}

		public Service Service { get; set; }

		private bool CanCreateProject()
		{
			return InputValidator.IsValidFolderName(Name) && InputValidator.IsValidPath(Location);
		}
	}
}