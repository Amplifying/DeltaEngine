using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Editor
{
	public class OnlineService : Service
	{
		public OnlineService()
		{
			AvailableProjects = new string[0];
			queuedContent = new List<string>();
			contentProjects = Settings.Current.GetValue("ContentProjects",
				new Dictionary<string, string>());
		}

		public string[] AvailableProjects { get; private set; }
		private readonly List<string> queuedContent;
		private readonly Dictionary<string, string> contentProjects;

		public void Connect(string userName, OnlineServiceConnection connection)
		{
			UserName = userName;
			onlineServiceConnection = connection;
			connection.DataReceived += OnDataReceived;
			send = connection.Send;
			var oldResolver = ContentLoader.resolver;
			ContentLoader.DisposeIfInitialized();
			ContentLoader.resolver = oldResolver;
			ContentLoader.Use<EditorContentLoader>();
			editorContent = new EditorContentLoader(onlineServiceConnection);
			editorContent.ContentUpdated += OnContentUpdated;
			editorContent.ContentDeleted += OnContentDeleted;
			editorContent.ContentReady += OnContentReady;
		}

		public string UserName { get; private set; }
		private OnlineServiceConnection onlineServiceConnection;

		private void OnDataReceived(object message)
		{
			var newProject = message as SetProject;
			if (newProject != null)
				ChangeProject(newProject);
			else if (DataReceived != null)
				DataReceived(message);
		}

		private void ChangeProject(SetProject project)
		{
			IsDeveloper = project.Permissions == ProjectPermissions.Full;
			if (project.Permissions == ProjectPermissions.None)
			{
				if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
					throw new NoAccessForProject(project.Name);
				MessageBox.Show("No access to project " + project.Name, "Fatal Error");
			}
			else
			{
				ProjectName = project.Name;
				Permissions = project.Permissions;
				editorContent.SetProject(project);
				if (ProjectChanged != null)
					ProjectChanged();
			}
		}

		public bool IsDeveloper { get; private set; }
		public string ProjectName { get; private set; }
		public ProjectPermissions Permissions { get; private set; }
		private EditorContentLoader editorContent;
		public event Action ProjectChanged;
		public event Action<object> DataReceived;
		private Action<object, bool> send;

		private void OnContentUpdated(ContentType type, string name)
		{
			editorContent.RefreshMetaData();
			if (ContentUpdated != null)
				ContentUpdated(type, name);
		}

		public event Action<ContentType, string> ContentUpdated;

		private void OnContentDeleted(string contentName)
		{
			editorContent.RefreshMetaData();
			if (ContentDeleted != null)
				ContentDeleted(contentName);
		}

		public event Action<string> ContentDeleted;

		private void OnContentReady()
		{
			if (ContentReady != null)
				ContentReady();
		}

		public event Action ContentReady;

		public void ChangeProject(string newProjectName)
		{
			if (newProjectName.Compare(ProjectName))
				return;
			if (!AvailableProjects.Contains(newProjectName))
				throw new ProjectNotAvailable(newProjectName);
			send(new ChangeProjectRequest(newProjectName), true);
		}

		public string CurrentContentProjectSolutionFilePath
		{
			get { return GetAbsoluteSolutionFilePath(ProjectName); }
			set { SetContentProjectSolutionFilePath(ProjectName, value); }
		}

		public string GetAbsoluteSolutionFilePath(string contentProjectName)
		{
			if (String.IsNullOrWhiteSpace(contentProjectName))
				return "";
			if (contentProjects.ContainsKey(contentProjectName))
				return contentProjects[contentProjectName];
			if (IsStarterKit(contentProjectName))
				return SolutionExtensions.GetSamplesSolutionFilePath();
			if (IsTutorial(contentProjectName))
				return SolutionExtensions.GetTutorialsSolutionFilePath();
			return "";
		}

		private static bool IsStarterKit(string projectName)
		{
			var starterKits = new[]
			{
				"Asteroids", "Blocks", "Breakout", "Drench", "EmptyApp", "GameOfDeath", "GhostWars",
				"Insight", "LogoApp", "SideScroller", "Snake"
			};
			return starterKits.Contains(projectName);
		}

		private static bool IsTutorial(string projectName)
		{
			return projectName == "DeltaEngine.Tutorials";
		}

		public void SetContentProjectSolutionFilePath(string name, string slnFilePath)
		{
			if (name == null)
				return;
			if (contentProjects.ContainsKey(name))
				contentProjects[name] = slnFilePath;
			else
				contentProjects.Add(name, slnFilePath);
			Settings.Current.SetValue("ContentProjects", contentProjects);
		}

		public void Send(object message, bool allowToCompressMessage = true)
		{
			send(message, allowToCompressMessage);
		}

		public IEnumerable<string> GetAllContentNames()
		{
			return editorContent.GetAllNames();
		}

		public IEnumerable<string> GetAllContentNamesByType(ContentType type)
		{
			return editorContent.GetAllNamesByType(type);
		}

		public ContentType? GetTypeOfContent(string content)
		{
			return editorContent.GetTypeOfContent(content);
		}

		public void UploadContent(ContentMetaData metaData,
			Dictionary<string, byte[]> optionalFileData = null)
		{
			editorContent.Upload(metaData, optionalFileData);
		}

		public void DeleteContent(string contentName, bool deleteSubContent = false)
		{
			editorContent.Delete(contentName, deleteSubContent);
		}

		public void StartPlugin(Type plugin)
		{
			if (StartEditorPlugin != null)
				StartEditorPlugin(plugin);
		}

		public event Action<Type> StartEditorPlugin;
		public EditorOpenTkViewport Viewport { get; set; }

		void Service.ShowToolbox(bool showToolbox)
		{
			UpdateToolboxVisibility(showToolbox);
		}

		public event Action<bool> UpdateToolboxVisibility;

		public void SetAvailableProjects(string[] projectNames)
		{
			AvailableProjects = projectNames;
			if (AvailableProjectsChanged != null)
				AvailableProjectsChanged();
		}

		public event Action AvailableProjectsChanged;

		public void UploadMutlipleContentToServer(string[] newFiles)
		{
			files = newFiles;
			if (isUploadingContent)
			{
				foreach (var file in files)
					queuedContent.Add(file);
				return;
			}
			UploadToOnlineService(files[0]);
			isUploadingContent = true;
			contentUploadIndex++;
			ContentUpdated += UploadNextFile;
		}

		private bool isUploadingContent;
		private int contentUploadIndex;

		private void UploadNextFile(ContentType arg1, string arg2)
		{
			if (files.Length < contentUploadIndex + 1)
			{
				contentUploadIndex = 0;
				if (queuedContent.Count == 0)
				{
					ContentUpdated -= UploadNextFile;
					isUploadingContent = false;
				}
				else
				{
					files = queuedContent.ToArray();
					queuedContent.Clear();
				}
			}
			else
			{
				UploadToOnlineService(files[contentUploadIndex]);
				contentUploadIndex++;
			}
		}

		private string[] files;

		public void UploadToOnlineService(string contentFilePath)
		{
			byte[] bytes;
			try
			{
				bytes = File.ReadAllBytes(contentFilePath);
			}
			catch (Exception)
			{
				Logger.Warning("Unable to read bytes for uploading to the server : " +
					Path.GetFileName(contentFilePath));
				return;
			}
			if (bytes.Length > MaximumFileSize)
			{
				Logger.Warning("The file you added is too large, the maximum file size is 16MB");
				return;
			}
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			fileNameAndBytes.Add(Path.GetFileName(contentFilePath), bytes);
			var metaDataCreator = new ContentMetaDataCreator();
			var contentMetaData = metaDataCreator.CreateMetaDataFromFile(contentFilePath);
			UploadContent(contentMetaData, fileNameAndBytes);
		}

		private const int MaximumFileSize = 16777216;
	}
}