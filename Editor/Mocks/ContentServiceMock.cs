using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Mocks
{
	public class ContentServiceMock : ContentService
	{
		/// <summary>
		/// Service used to test the contentmanager
		/// </summary>
		public ContentServiceMock(IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
		}

		private readonly IFileSystem fileSystem;

		public void AddProject(string projectName)
		{
			fileSystem.Directory.CreateDirectory(projectName);
		}

		public List<string> GetProjects()
		{
			List<string> projectList = new List<string>();
			projectList.Add("Content");
			projectList.Add("BreakOut");
			fileSystem.Directory.CreateDirectory("BreakOut");
			return projectList;
		}

		public List<string> GetContentNames(string projectName)
		{
			List<string> contentList = new List<string>();
			contentList.Add("DeltaEngineLogo.png");
			contentList.Add("Ball.png");
			return contentList;
		}

		public void AddContent(string projectName, string contentName, Stream data)
		{
			fileSystem.File.Copy(@"Content\DeltaEngineLogo.png", @"BreakOut\DeltaEngineLogo.png");
		}

		public void DeleteContent(string projectName, string contentName)
		{
			
		}

		public Stream LoadContent(string projectName, string contentName)
		{
			var fullFilename = Path.Combine(projectName, contentName);
			if (!fileSystem.File.Exists(fullFilename))
				throw new FileNotFoundException(fullFilename);

			return File.OpenRead(fullFilename);
		}

		public void SaveImagesAsAnimation(List<string> itemList, string animationName, string projectName)
		{
		
		}
	}
}
