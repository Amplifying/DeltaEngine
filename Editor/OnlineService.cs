using System;
using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Editor
{
	public class OnlineService : Service
	{
		public void CreateInitialContentLoader(OnlineServiceConnection connection)
		{
			editorContent = new EditorContentLoader(connection);
		}

		private EditorContentLoader editorContent;

		public void Connect(string userName, OnlineServiceConnection connection)
		{
			onlineServiceConnection = connection;
			connection.DataReceived += OnDataReceived;
			send = connection.Send;
			UserName = userName;
		}

		private OnlineServiceConnection onlineServiceConnection;

		private void OnDataReceived(object message)
		{
			var login = message as LoginSuccessful;
			var newProject = message as SetProject;
			if (login != null)
				UserName = login.UserName;
			else if (newProject != null)
				ChangeProject(newProject);
			else if (message.GetType() == typeof(ServerError))
				Logger.Warning(message.ToString());
			else if (DataReceived != null)
				DataReceived(message);
		}

		private Action<object> send;

		public string UserName { get; private set; }

		private void ChangeProject(SetProject project)
		{
			if (project.Permissions == ProjectPermissions.None)
				MessageBox.Show("No access to project " + project.ProjectName, "Fatal Error");
			else
			{
				ProjectName = project.ProjectName;
				Permissions = project.Permissions;
				var contentDataResolver = editorContent.resolver;
				editorContent.ContentChanged -= OnContentChanged;
				editorContent.Dispose();
				editorContent = new EditorContentLoader(onlineServiceConnection, project);
				editorContent.resolver = contentDataResolver;
				if (ContentChanged != null)
					ContentChanged();
				editorContent.ContentChanged += OnContentChanged;
			}
		}

		public string ProjectName { get; private set; }
		public ProjectPermissions Permissions { get; private set; }
		public event Action<object> DataReceived;

		private void OnContentChanged()
		{
			if (ContentChanged != null)
				ContentChanged();
		}

		public event Action ContentChanged;

		public void RequestChangeProject(string newProjectName)
		{
			if (newProjectName.Compare(ProjectName))
				return;
			send(new ChangeProjectRequest(newProjectName));
		}

		public void Send(object message)
		{
			send(message);
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

		public void DeleteContent(string contentName)
		{
			editorContent.Delete(contentName);
		}
	}
}