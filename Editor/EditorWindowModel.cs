using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DeltaEngine.Editor.Common;
using DeltaEngine.Networking.Sockets;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace DeltaEngine.Editor
{
	public class EditorWindowModel : ViewModelBase
	{
		public EditorWindowModel()
			: this(new EditorPluginLoader()) {}

		public EditorWindowModel(EditorPluginLoader plugins)
		{
			this.plugins = plugins;
			RegisterCommands();
			Error = "";
			LoadApiKeyAndTryToLogin();
			service.Connection.Connected += UpdateLog;
			EditorPlugins = new List<EditorPluginView>();
		}

		private readonly EditorPluginLoader plugins;

		private void RegisterCommands()
		{
			OnApiKeyChanged = new RelayCommand<string>(ChangeApiKey);
			OnLoginButtonClicked = new RelayCommand(ValidateLogin);
			OnGetApiKeyClicked = new RelayCommand(OpenLoginWebsite);
			OnLogoutButtonClicked = new RelayCommand(Logout);
		}

		public ICommand OnApiKeyChanged { get; private set; }

		private void ChangeApiKey(string key)
		{
			ApiKey = key;
		}

		public string ApiKey
		{
			get { return apiKey; }
			private set
			{
				apiKey = value;
				RaisePropertyChanged("ApiKey");
			}
		}
		private string apiKey;

		public ICommand OnLoginButtonClicked { get; private set; }
		
		public bool IsLoggedIn
		{
			get { return isLoggedIn; }
			set
			{
				isLoggedIn = value;
				UpdateLog();
				RaisePropertyChanged("LoginPanelVisibility");
				RaisePropertyChanged("EditorPanelVisibility");
				RaisePropertyChanged("IsLoggedIn");
				if (IsLoggedIn)
					SaveApiKey();
			}
		}
		private bool isLoggedIn;

		private void UpdateLog()
		{
			Error = !service.Connection.IsConnected ?
				"You are not connected to DeltaEngine.net, please check your internet connection!" :
				!IsLoggedIn ? "Please obtain your API Key and login." : "";
			RaisePropertyChanged("Error");
			RaisePropertyChanged("ErrorVisibility");
		}

		public string Error { get; private set; }

		public Visibility ErrorVisibility
		{
			get { return Error != "" ? Visibility.Visible : Visibility.Hidden; }
		}

		public Visibility LoginPanelVisibility
		{
			get { return IsLoggedIn ? Visibility.Hidden : Visibility.Visible; }
		}

		public Visibility EditorPanelVisibility
		{
			get { return IsLoggedIn ? Visibility.Visible : Visibility.Hidden; }
		}

		public void SaveApiKey()
		{
			//TODO: provide settings load/save functionality in Common
			var registryKey = Registry.CurrentUser.CreateSubKey(@"Software\DeltaEngine\Editor");
			if (registryKey != null && ApiKey != null)
				registryKey.SetValue("ApiKey", ApiKey);
		}

		private void LoadApiKey()
		{
			var registryKey = Registry.CurrentUser.OpenSubKey(@"Software\DeltaEngine\Editor", false);
			if (registryKey != null)
				ApiKey = (string)registryKey.GetValue("ApiKey");
		}

		public void LoadApiKeyAndTryToLogin()
		{
			LoadApiKey();
			ValidateLogin();
		}

		private void ValidateLogin()
		{
			if (!service.Connection.IsConnected)
				UpdateLog();
			else
				service.Login(ApiKey, successfullyLoggedIn => IsLoggedIn = successfullyLoggedIn);
		}

		private readonly OnlineService service = new OnlineService(new TcpSocket());

		public ICommand OnGetApiKeyClicked { get; private set; }
		public ICommand OnLogoutButtonClicked { get; private set; }

		private void Logout()
		{
			ApiKey = "";
			SaveApiKey();
			IsLoggedIn = false;
			RaisePropertyChanged("LoginPanelVisibility");
			RaisePropertyChanged("EditorPanelVisibility");
		}

		private static void OpenLoginWebsite()
		{
			Process.Start("http://deltaengine.net/Account/ApiKey");
		}

		public List<EditorPluginView> EditorPlugins { get; private set; }

		public void AddAllPlugins()
		{
			foreach (var pluginType in plugins.UserControlsType)
			{
				var instance = Activator.CreateInstance(pluginType, service) as EditorPluginView;
				if (instance == null)
					return;

				InsertPluginAtRightPriority(instance);
			}
		}

		private void InsertPluginAtRightPriority(EditorPluginView instance)
		{
			for (int index = 0; index < EditorPlugins.Count; index++)
			{
				if (EditorPlugins[index].Priority <= instance.Priority)
					continue;
				EditorPlugins.Insert(0, instance);
				break;
			}
			if (!EditorPlugins.Contains(instance))
				EditorPlugins.Add(instance);
		}
	}
}