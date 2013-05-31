using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Mocks
{
	/// <summary>
	/// ContentManager implementation with local files on disk
	/// </summary>
	public class ContentServiceFiles : ContentService
	{
		public ContentServiceFiles(IFileSystem fileSystem)
		{
			ProjectList = new List<string>();
			ImageList = new List<string>();
			xmlFile = new ContentMetaDataCreator(this);
			this.fileSystem = fileSystem;
			if (!fileSystem.Directory.Exists(ContentPath))
				fileSystem.Directory.CreateDirectory(ContentPath);
		}

		public List<string> ProjectList { get; set; }
		public List<string> ImageList { get; set; }
		private readonly ContentMetaDataCreator xmlFile;
		public readonly IFileSystem fileSystem;

		public void AddProject(string projectName)
		{
			if (string.IsNullOrEmpty(projectName))
				return;

			string newProject = Path.Combine(ContentPath, projectName);
			if (fileSystem.Directory.Exists(newProject))
				return;

			fileSystem.Directory.CreateDirectory(newProject);
		}

		public List<string> GetProjects()
		{
			ProjectList.Clear();
			string[] projects = fileSystem.Directory.GetDirectories(ContentPath);
			foreach (var project in projects)
				ProjectList.Add(Path.GetFileName(project));
			return ProjectList;
		}

		public const string ContentPath = "Content";

		public List<string> GetContentNames(string projectName)
		{
			ImageList.Clear();
			string[] imageList = fileSystem.Directory.GetFiles(Path.Combine(ContentPath, projectName));
			foreach (var imagePath in imageList)
				if (!imagePath.Contains("ContentMetaData.xml"))
					ImageList.Add(Path.GetFileName(imagePath));
			xmlFile.SaveImagesToXml(projectName);
			return ImageList;
		}
		
		public void AddContent(string projectName, string contentName, Stream data)
		{
			var targetFilePath = Path.Combine(ContentPath, projectName, contentName);
			using (var targetStream = fileSystem.File.Create(targetFilePath))
			{
				data.CopyTo(targetStream);
				ImageList.Add(contentName);
			}
		}

		public void DeleteContent(string projectName, string contentName)
		{
			if (projectName == null || contentName == null)
				return;

			ImageList.Remove(contentName);
			fileSystem.File.Delete(Path.Combine(ContentPath, projectName, contentName));
			xmlFile.SaveImagesToXml(projectName);
		}

		public Stream LoadContent(string projectName, string contentName)
		{
			var fullFilename = Path.Combine(ContentPath, projectName, contentName);
			if (!fileSystem.File.Exists(fullFilename))
				throw new FileNotFoundException(fullFilename);

			return fileSystem.File.OpenRead(fullFilename);
		}

		public void SaveImagesAsAnimation(List<string> itemList, string animationName, string projectName)
		{
			xmlFile.SaveImagesAsAnimation(itemList, animationName, projectName);
		}
	}
}