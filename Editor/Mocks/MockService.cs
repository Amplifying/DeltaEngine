using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;

namespace DeltaEngine.Editor.Mocks
{
	public class MockService : Service
	{
		public MockService(string userName, string projectName)
		{
			UserName = userName;
			ProjectName = projectName;
			IsDeveloper = true;
			EditorSettings = new MockSettings();
			availableContentWithCodeProjects = new Dictionary<string, string>();
		}

		public string UserName { get; private set; }
		public string ProjectName { get; private set; }
		public bool IsDeveloper { get; private set; }
		public Settings EditorSettings { get; private set; }
		private readonly Dictionary<string, string> availableContentWithCodeProjects;

		public string[] AvailableProjects
		{
			get { return availableContentWithCodeProjects.Keys.ToArray(); }
		}
		
		public void ReceiveData(object data)
		{
			if (DataReceived != null)
				DataReceived(data);
			if (ContentUpdated != null)
				ContentUpdated(ContentType.Scene, "MockContent");
			if (ContentDeleted != null)
				ContentDeleted("MockContent");
		}

		public void RecieveData(ContentType type)
		{
			if (ContentUpdated != null)
				ContentUpdated(type, "MockContent");
		}

		public void ChangeProject(string projectName)
		{
			ProjectName = projectName;
			if (!String.IsNullOrEmpty(projectName))
				AddContentProject(projectName);
			if (ProjectChanged != null)
				ProjectChanged();
		}

		private void AddContentProject(string newContentProject)
		{
			if (availableContentWithCodeProjects.ContainsKey(newContentProject))
				return;
			availableContentWithCodeProjects.Add(newContentProject, "");
			RaiseAvailableProjectsChangedEvent();
		}

		private void RaiseAvailableProjectsChangedEvent()
		{
			if (AvailableProjectsChanged != null)
				AvailableProjectsChanged();
		}

		public event Action AvailableProjectsChanged;

		public event Action ProjectChanged;
		public event Action<object> DataReceived;
		public event Action<ContentType, string> ContentUpdated;
		public event Action<string> ContentDeleted;

		public void SetAvailableProjects(params string[] projectNames)
		{
			availableContentWithCodeProjects.Clear();
			foreach (string projectName in projectNames)
				AddContentProject(projectName);
			RaiseAvailableProjectsChangedEvent();
		}

		public void SetContentReady()
		{
			if (ContentReady != null)
				ContentReady();
		}

		public event Action ContentReady;

		public string CurrentContentProjectSolutionFilePath
		{
			get
			{
				return GetAbsoluteSolutionFilePath(ProjectName);
			}
			set
			{
				availableContentWithCodeProjects[ProjectName] = value;
			}
		}
		
		private string solutionFilePath;

		public string GetAbsoluteSolutionFilePath(string contentProjectName)
		{
			string customFilePath;
			if (!availableContentWithCodeProjects.TryGetValue(contentProjectName, out customFilePath))
				return "";
			if (!String.IsNullOrEmpty(customFilePath))
				return customFilePath;
			if (contentProjectName.Contains("Tutorials"))
				return SolutionExtensions.GetTutorialsSolutionFilePath();
			return SolutionExtensions.GetSamplesSolutionFilePath();
		}

		public void SetContentProjectSolutionFilePath(string name, string slnFilePath)
		{
			solutionFilePath = slnFilePath;
		}

		public virtual void Send(object message, bool allowToCompressMessage = true) {}

		public IEnumerable<string> GetAllContentNames()
		{
			var list = new List<string>();
			foreach (ContentType type in EnumExtensions.GetEnumValues<ContentType>())
				list.AddRange(GetAllContentNamesByType(type));
			return list;
		}

		public IEnumerable<string> GetAllContentNamesByType(ContentType type)
		{
			var list = new List<string>();
			for (int i = 0; i < 2; i++)
			{
				string contentName = "My" + type + (i + 1);
				if (type == ContentType.Material)
					contentName += "In" + (i + 2) + "D";
				list.Add(contentName);
			}
			return list;
		}

		public ContentType? GetTypeOfContent(string content)
		{
			if ("TestUser" == content)
				return null;
			if (content.Contains("ImageAnimation"))
				return ContentType.ImageAnimation;
			if (content.Contains("SpriteSheet"))
				return ContentType.SpriteSheetAnimation;
			if (content.Contains("Mesh"))
				return ContentType.Mesh;
			if (content.Contains("Material"))
				return ContentType.Material;
			if (content.Contains("UI") || content.Contains("Scene"))
				return ContentType.Scene;
			if (content.Contains("Theme"))
				return ContentType.Theme;
			return ContentType.Image;
		}

		public void UploadContent(ContentMetaData metaData,
			Dictionary<string, byte[]> optionalFileData = null)
		{
			NumberOfMessagesSent++;
		}

		public void UploadMutlipleContentToServer(string[] files) {}

		public int NumberOfMessagesSent { get; private set; }

		public void DeleteContent(string selectedContent, bool deleteSubContent)
		{
			NumberOfMessagesSent++;
		}

		public void StartPlugin(Type plugin) {}

		public EditorOpenTkViewport Viewport { get; set; }

		void Service.ShowToolbox(bool showToolbox) {}
	}
}