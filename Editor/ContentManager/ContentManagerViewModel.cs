using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using DeltaEngine.Editor.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// passes the data used in the ContentManagerView
	/// </summary>
	public sealed class ContentManagerViewModel : ViewModelBase
	{
		public ContentManagerViewModel(ContentService content)
		{
			this.content = content;
			content.GetProjects();
			RaisePropertyChanged("Projects");
			Messenger.Default.Register<string>(this, "DeletingImage", DeleteImageFromList);
			Messenger.Default.Register<IDataObject>(this, "AddImage", DropContent);
			Messenger.Default.Register<string>(this, "AddProject", AddNewProject);
			Images = new ObservableCollection<string>();
		}

		private readonly ContentService content;
		public ObservableCollection<string> Images { get; set; }

		public void DropContent(IDataObject dropObject)
		{
			if (!dropObject.GetDataPresent(DataFormats.FileDrop))
				return;

			var files = (string[])dropObject.GetData(DataFormats.FileDrop);
			var imageFilePath = files[0];
			using (Stream stream = File.OpenRead(imageFilePath))
			{
				content.AddContent(SelectedProject, Path.GetFileName(imageFilePath), stream);
				Images = new ObservableCollection<string>(content.GetContentNames(SelectedProject));
			}
			RaisePropertyChanged("Images");
		}

		public string SelectedProject
		{
			get { return selectedProject; }
			set { ChangeSelectedProject(value); }
		}

		private string selectedProject;

		private void ChangeSelectedProject(string value)
		{
			selectedProject = value;
			if (selectedProject != null)
				Images = new ObservableCollection<string>(content.GetContentNames(selectedProject));
			RaisePropertyChanged("Images");
			ViewImage = null;
			RaisePropertyChanged("ViewImage");
			EditViewImage();
		}

		public BitmapImage ViewImage { get; private set; }

		public void EditViewImage()
		{
			if (selectedContent == null)
				return;	

			var image = new BitmapImage();
			using (var contentStream = content.LoadContent(SelectedProject, SelectedContent))
			{
				image.BeginInit();
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.StreamSource = contentStream;
				image.EndInit();
			}
			ViewImage = image;
			RaisePropertyChanged("ViewImage");
		}

		public string SelectedContent
		{
			get { return selectedContent; }
			set
			{
				selectedContent = value;
				EditViewImage();
			}
		}

		private string selectedContent;

		public ObservableCollection<string> Projects
		{
			get { return new ObservableCollection<string>(content.GetProjects()); }
		}

		public void DeleteImageFromList(string msg)
		{
			ViewImage = null;
			RaisePropertyChanged("ViewImage");
			string deleteContent = selectedContent;
			Images.Remove(selectedContent);
			content.DeleteContent(SelectedProject, deleteContent);
		}

		public void AddNewProject(string obj)
		{
			if (string.IsNullOrEmpty(NewProjectName))
			{
				MessageBox.Show("Please type in a project name", "Warning");
				return;
			}

			content.AddProject(NewProjectName);
			content.GetProjects();
			RaisePropertyChanged("Projects");
		}

		public string NewProjectName { get; set; }
	}
}