using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DeltaEngine.Core;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Core.Properties;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Logging;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;
using DeltaEngine.Platforms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace DeltaEngine.Editor
{
	public class EditorViewModel : ViewModelBase
	{
		public EditorViewModel()
			: this(new EditorPluginLoader(), new FileSettings()) {}

		public EditorViewModel(EditorPluginLoader plugins, Settings settings)
		{
			this.plugins = plugins;
			this.settings = settings;
			service = new OnlineService(settings);
			AvailableProjects = new List<ProjectNameAndFontWeight>();
			Error = Resources.EnterYourApiKey;
			SetupLogger();
			SetVersionNumber();
			plugins.FindAndLoadAllPlugins();
			RegisterCommands();
			SetApiKey(LoadDataFromRegistry("ApiKey"));
			SetCurrentProject(LoadDataFromRegistry("SelectedProject"));
			ConnectToOnlineServiceAndTryToLogin();
			EditorPlugins = new List<EditorPluginView>();
			messageViewModel = new PopupMessageViewModel(service);
			messageViewModel.MessageUpdated += RaisePopupMessageProperties;
		}

		private readonly EditorPluginLoader plugins;
		private readonly Settings settings;
		private readonly OnlineService service;
		public List<ProjectNameAndFontWeight> AvailableProjects { get; private set; }

		public string Error
		{
			get { return error; }
			private set
			{
				error = value;
				RaisePropertyChanged("Error");
				RaisePropertyChanged("ErrorForegroundColor");
				RaisePropertyChanged("ErrorBackgroundColor");
			}
		}

		private string error;

		private void SetupLogger()
		{
			TextLogger = new TextLogger();
			TextLogger.NewLogMessage += () => RaisePropertyChanged("TextLogger");
			TextLogger.Write(Logger.MessageType.Info, "Welcome to the Delta Engine Editor");
		}

		public TextLogger TextLogger { get; set; }

		private void SetVersionNumber()
		{
			VersionNumber = "Version: " + Assembly.GetExecutingAssembly().GetName().Version;
		}

		public string VersionNumber { get; set; }

		private void RegisterCommands()
		{
			OnLoginButtonClicked = new RelayCommand(ValidateLogin);
			OnLogoutButtonClicked = new RelayCommand(Logout);
		}

		public ICommand OnLoginButtonClicked { get; private set; }
		public ICommand OnLogoutButtonClicked { get; private set; }

		private void SetApiKey(string apiKey)
		{
			ApiKey = apiKey;
			RaisePropertyChanged("ApiKey");
		}

		public string ApiKey { get; set; }

		private static string LoadDataFromRegistry(string name)
		{
			using (var key = Registry.CurrentUser.OpenSubKey(RegistryPathForEditorValues, false))
				if (key != null)
					return (string)key.GetValue(name);
			return null;
		}

		private const string RegistryPathForEditorValues = @"Software\DeltaEngine\Editor";

		private void SetCurrentProject(string project)
		{
			SelectedProject = new ProjectNameAndFontWeight(project, FontWeights.Normal);
			RaisePropertyChanged("SelectedProject");
		}

		private void ConnectToOnlineServiceAndTryToLogin()
		{
			if (tryingToConnect)
				return;
			tryingToConnect = true;
			connection = new OnlineServiceConnection();
			connection.Connected += ValidateLogin;
			connection.DataReceived += OnDataReceived;
			connection.Connect(settings.OnlineServiceIp, settings.OnlineServicePort, OnTimeout);
		}

		private bool tryingToConnect;
		private OnlineServiceConnection connection;

		private void OnTimeout()
		{
			Disconnect();
			Error = Resources.ConnectionToDeltaEngineTimedOut;
		}

		public OnlineService Service
		{
			get { return service; }
		}

		private void ValidateLogin()
		{
			if (!connection.IsConnected)
				ConnectToOnlineServiceAndTryToLogin();
			else if (!string.IsNullOrEmpty(ApiKey))
				connection.Send(new LoginRequest(ApiKey, SelectedProject.Name));
			else
				Error = Resources.GetApiKeyHere;
		}

		public ProjectNameAndFontWeight SelectedProject
		{
			get
			{
				if (string.IsNullOrEmpty(selectedProject.Name))
					selectedProject = new ProjectNameAndFontWeight(DefaultProject, FontWeights.Normal);
				return selectedProject;
			}
			set
			{
				if (isLoggedIn)
					service.RequestChangeProject(value.Name);
				selectedProject = value;
				SaveCurrentProject();
			}
		}

		private ProjectNameAndFontWeight selectedProject;
		private const string DefaultProject = "GhostWars";

		private void OnDataReceived(object message)
		{
			var serverError = message as ServerError;
			var projectNames = message as ProjectNamesResult;
			var loginMessage = message as LoginSuccessful;
			var newProject = message as SetProject;
			var contentReady = message as ContentReady;
			if (serverError != null)
			{
				Error = "Server Error: " + serverError.Error;
				Logger.Warning(serverError.Error);
			}
			else if (projectNames != null)
				RefreshAvailableProjects(projectNames);
			else if (loginMessage != null)
				Login(loginMessage);
			else if (newProject != null)
				IsContentReady = false;
			else if (contentReady != null)
				IsContentReady = true;
		}

		private void RefreshAvailableProjects(ProjectNamesResult projectNames)
		{
			AvailableProjects.Clear();
			var projectNamesAndWeight = new List<ProjectNameAndFontWeight>();
			var fontWeight = FontWeights.Bold;
			string tutorials = "";
			foreach (var projectName in projectNames.ProjectNames)
			{
				if (projectName == "Asteroids")
					fontWeight = FontWeights.Normal;
				if (projectName == "DeltaEngine.Tutorials")
				{
					tutorials = projectName;
					continue;
				}
				projectNamesAndWeight.Add(new ProjectNameAndFontWeight(projectName, fontWeight));
			}
			AvailableProjects.AddRange(projectNamesAndWeight);
			if (!string.IsNullOrEmpty(tutorials))
				AvailableProjects.Add(new ProjectNameAndFontWeight(tutorials, fontWeight));
			RaisePropertyChanged("AvailableProjects");
		}

		public class ProjectNameAndFontWeight
		{
			public ProjectNameAndFontWeight(string name, FontWeight weight)
			{
				Name = name;
				Weight = weight;
			}

			public string Name { get; private set; }
			public FontWeight Weight { get; private set; }
		}

		private void Login(LoginSuccessful loginMessage)
		{
			connection.Send(new ProjectNamesRequest());
			Service.Connect(loginMessage.UserName, connection);
			SaveApiKey();
			SaveCurrentProject();
			IsLoggedIn = true;
			ChangeVisiblePanel();
			Error = "";
			RaisePropertyChanged("Service");
		}

		public void SaveApiKey()
		{
			SaveDataInRegistry("ApiKey", ApiKey);
		}

		private static void SaveDataInRegistry(string name, string data)
		{
			if (data == null)
				return;
			using (var registryKey = Registry.CurrentUser.CreateSubKey(RegistryPathForEditorValues))
				if (registryKey != null)
					registryKey.SetValue(name, data);
		}

		private void SaveCurrentProject()
		{
			SaveDataInRegistry("SelectedProject", SelectedProject.Name);
		}

		public bool IsLoggedIn
		{
			get { return isLoggedIn; }
			private set
			{
				isLoggedIn = value;
				RaisePropertyChanged("IsLoggedIn");
			}
		}

		private bool isLoggedIn;

		private void ChangeVisiblePanel()
		{
			RaisePropertyChanged("LoginPanelVisibility");
			RaisePropertyChanged("EditorPanelVisibility");
		}

		public bool IsContentReady
		{
			get { return isContentReady; }
			set
			{
				isContentReady = value;
				RaisePropertyChanged("IsContentReady");
			}
		}

		private bool isContentReady;

		private void Disconnect()
		{
			tryingToConnect = false;
			connection.Dispose();
			connection = new OnlineServiceConnection();
		}

		private void Logout()
		{
			SetApiKey("");
			SaveApiKey();
			SaveCurrentProject();
			IsLoggedIn = false;
			Error = Resources.EnterYourApiKey;
			ChangeVisiblePanel();
			Disconnect();
		}

		public Brush ErrorForegroundColor
		{
			get { return GetErrorBrushColor().Item1; }
		}

		private Tuple<Brush, Brush> GetErrorBrushColor()
		{
			if (Error == Resources.ConnectionToDeltaEngineTimedOut)
				return new Tuple<Brush, Brush>(Brushes.White, Brushes.DarkRed);
			if (Error == Resources.GetApiKeyHere)
				return new Tuple<Brush, Brush>(Brushes.Blue, Brushes.Transparent);
			return new Tuple<Brush, Brush>(Brushes.Black, Brushes.Transparent);
		}

		public Brush ErrorBackgroundColor
		{
			get { return GetErrorBrushColor().Item2; }
		}

		public Visibility LoginPanelVisibility
		{
			get { return IsLoggedIn ? Visibility.Hidden : Visibility.Visible; }
		}

		public Visibility ErrorVisibility
		{
			get { return Error != "" ? Visibility.Visible : Visibility.Hidden; }
		}

		public Visibility EditorPanelVisibility
		{
			get { return IsLoggedIn ? Visibility.Visible : Visibility.Hidden; }
		}

		public List<EditorPluginView> EditorPlugins { get; private set; }

		private readonly PopupMessageViewModel messageViewModel;

		private void RaisePopupMessageProperties()
		{
			RaisePropertyChanged("PopupText");
			RaisePropertyChanged("PopupVisibility");
		}

		public string PopupText
		{
			get { return messageViewModel.Text; }
		}

		public Visibility PopupVisibility
		{
			get { return messageViewModel.Visiblity; }
		}

		public void AddAllPlugins()
		{
			foreach (var pluginType in plugins.UserControlsType)
				TryCreatePlugin(pluginType);
		}

		private void TryCreatePlugin(Type pluginType)
		{
			try
			{
				CreatePlugin(pluginType);
			}
			catch (MissingMethodException ex)
			{
				Logger.Error(new EditorPluginViewHasNoParameterlessConstructor(pluginType.ToString(), ex));
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		private void CreatePlugin(Type pluginType)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var instance = Activator.CreateInstance(pluginType) as EditorPluginView;
			stopwatch.Stop();
			if (stopwatch.ElapsedMilliseconds > 50 && instance != null)
				Logger.Warning("Initialization of plugin " + instance.ShortName + " took to long: " +
					stopwatch.ElapsedMilliseconds + "ms");
			if (instance != null)
				InsertPluginAtRightPosition(instance);
		}

		private void InsertPluginAtRightPosition(EditorPluginView instance)
		{
			if (EditorPlugins.Contains(instance))
				return;
			if (instance.GetType().Name == "SampleBrowserView" ||
				instance.GetType().Name == "ProjectCreatorView")
				EditorPlugins.Insert(0, instance);
			else
				EditorPlugins.Add(instance);
		}

		private class EditorPluginViewHasNoParameterlessConstructor : Exception
		{
			public EditorPluginViewHasNoParameterlessConstructor(string plugin, Exception inner)
				: base("The Editor Plugin " + plugin + " is missing a parameterless constructor.", inner) {}
		}

		public bool StartEditorMaximized
		{
			get
			{
				var data = LoadDataFromRegistry(StartMaximized);
				return data == null || Convert.ToBoolean(data);
			}
			set { SaveDataInRegistry(StartMaximized, Convert.ToString(value)); }
		}

		private const string StartMaximized = "StartMaximized";

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
			service.UploadContent(contentMetaData, fileNameAndBytes);
		}

		private const int MaximumFileSize = 16777216;

		public void SetProjectAndTest(string initialPath, string projectName, string testName)
		{
			//Logger.Info("Path: " + initialPath + ", Project: " + projectName + ", Test: " + testName);
		}
	}
}