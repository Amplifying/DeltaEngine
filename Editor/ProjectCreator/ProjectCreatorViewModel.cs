using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.ProjectCreator
{
	public class ProjectCreatorViewModel : ViewModelBase
	{
		public ProjectCreatorViewModel()
		{
			Project = new CsProject();
			AvailableFrameworks = CreateListFromFrameworkEnum();
			RegisterCommands();
		}

		private CsProject Project { get; set; }

		public List<DeltaEngineFramework> AvailableFrameworks { get; private set; }

		private static List<DeltaEngineFramework> CreateListFromFrameworkEnum()
		{
			var arrayOfFrameworkEnum = Enum.GetValues(typeof(DeltaEngineFramework));
			var enumerableEnum = arrayOfFrameworkEnum.Cast<DeltaEngineFramework>();
			return new List<DeltaEngineFramework>(enumerableEnum);
		}

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
			get { return Project.Name; }
			private set
			{
				Project.Name = value;
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
			get { return Project.Framework; }
			private set
			{
				Project.Framework = value;
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
			get { return Project.Location; }
			private set
			{
				Project.Location = value;
				RaisePropertyChanged("Location");
			}
		}

		public ICommand OnCreateClicked { get; private set; }

		private void CreateProject()
		{
			var projectCreator = new ProjectCreator(Project, VsTemplate.GetEmptyGame(), new FileSystem());
			projectCreator.CreateProject();
			if (projectCreator.HaveTemplateFilesBeenCopiedToLocation())
				MessageBox.Show("Project has successfully been created.", "Project created");
			else
				MessageBox.Show(
					"Project has not been created. Please make sure the specified location is available.",
					"Error");
		}

		private bool CanCreateProject()
		{
			return InputValidator.IsValidFolderName(Name) && InputValidator.IsValidPath(Location);
		}
	}
}