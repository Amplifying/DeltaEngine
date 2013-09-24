using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DeltaEngine.Content;
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
			AvailableProjects = new List<string>();
			Error = Resources.EnterYourApiKey;
			SetupLogger();
			plugins.FindAndLoadAllPlugins();
			RegisterCommands();
			SetApiKey(LoadDataFromRegistry("ApiKey"));
			SetCurrentProject(LoadDataFromRegistry("SelectedProject"));
			ConnectToOnlineServiceAndTryToLogin();
			EditorPlugins = new List<EditorPluginView>();
		}

		private readonly EditorPluginLoader plugins;
		private readonly Settings settings;
		public List<string> AvailableProjects { get; private set; }

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
			SelectedProject = project;
			RaisePropertyChanged("SelectedProject");
		}

		private void ConnectToOnlineServiceAndTryToLogin()
		{
			if (tryingToConnect)
				return;
			tryingToConnect = true;
			connection = new OnlineServiceConnection();
			connection.Connected += () => connection.Send(new ProjectNamesRequest());
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
		private readonly OnlineService service = new OnlineService();

		private void ValidateLogin()
		{
			if (!connection.IsConnected)
				ConnectToOnlineServiceAndTryToLogin();
			else if (!string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(SelectedProject))
				connection.Send(new LoginRequest(ApiKey, SelectedProject));
			else if (string.IsNullOrEmpty(ApiKey))
				Error = Resources.GetApiKeyHere;
			else
				Error = Resources.ObtainApiKeyAndSelectProject;
		}

		public string SelectedProject
		{
			get { return selectedProject; }
			set
			{
				if (isLoggedIn)
					service.RequestChangeProject(value);
				selectedProject = value;
				SaveCurrentProject();
			}
		}
		private string selectedProject;

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
			{
				AvailableProjects.Clear();
				AvailableProjects.AddRange(projectNames.ProjectNames);
				RaisePropertyChanged("AvailableProjects");
			}
			else if (loginMessage != null)
				Login(loginMessage);
			else if (newProject != null)
				IsContentReady = false;
			else if (contentReady != null)
				IsContentReady = true;
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
			SaveDataInRegistry("SelectedProject", SelectedProject);
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
				Logger.Warning("Unknown content type was unable to be added to the server : " +
					Path.GetFileName(contentFilePath));
				return;
			}

			if (bytes.Length > MaximumFileSize)
			{
				Logger.Warning("The file you wanted to add is too large, the maximum filesize is 16MB");
				return;
			}
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			fileNameAndBytes.Add(Path.GetFileName(contentFilePath), bytes);
			var metaDataCreator = new ContentMetaDataCreator();
			var contentMetaData = metaDataCreator.CreateMetaDataFromFile(contentFilePath);
			if (ContentLoader.Exists(Path.GetFileName(contentFilePath)))
				service.DeleteContent(Path.GetFileName(contentFilePath));
			service.UploadContent(contentMetaData, fileNameAndBytes);
		}

		private const int MaximumFileSize = 16777216;
	}
}