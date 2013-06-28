using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DeltaEngine.Editor.Common;
using DeltaEngine.Editor.Common.Properties;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Networking.Sockets;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace DeltaEngine.Editor
{
	public class EditorViewModel : ViewModelBase
	{
		public EditorViewModel()
			: this(new EditorPluginLoader()) {}

		public EditorViewModel(EditorPluginLoader plugins)
		{
			this.plugins = plugins;
			Error = "";
			RegisterCommands();
			LoadApiKey();
			ConnectToOnlineService();
			Service.Connection.Connected += ValidateLogin;
			EditorPlugins = new List<EditorPluginView>();
		}

		private readonly EditorPluginLoader plugins;

		public string Error { get; private set; }

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

		private void UpdateErrorMessage()
		{
			Error = !Service.Connection.IsConnected
				? Resources.NotConnectedToDeltaEngine : !isLoggedIn ? Resources.ObtainApiKey : "";
			RaisePropertyChanged("Error");
			RaisePropertyChanged("ErrorVisibility");
		}

		private bool isLoggedIn;

		public Visibility ErrorVisibility
		{
			get { return Error != "" ? Visibility.Visible : Visibility.Hidden; }
		}

		public Visibility LoginPanelVisibility
		{
			get { return isLoggedIn ? Visibility.Hidden : Visibility.Visible; }
		}

		public Visibility EditorPanelVisibility
		{
			get { return isLoggedIn ? Visibility.Visible : Visibility.Hidden; }
		}

		public void SaveApiKey()
		{
			if (ApiKey == null)
				return;

			var registryKey = Registry.CurrentUser.CreateSubKey(RegistryPathForApiKey);
			if (registryKey != null)
				registryKey.SetValue("ApiKey", ApiKey);
		}

		private const string RegistryPathForApiKey = @"Software\DeltaEngine\Editor";

		public void LoadApiKey()
		{
			var registryKey = Registry.CurrentUser.OpenSubKey(RegistryPathForApiKey, false);
			if (registryKey != null)
				ApiKey = (string)registryKey.GetValue("ApiKey");
		}

		private void ConnectToOnlineService()
		{
			Service = new OnlineService(new TcpSocket());
		}

		public OnlineService Service { get; private set; }

		private void ValidateLogin()
		{
			if (!Service.Connection.IsConnected)
				UpdateErrorMessage();
			else
				Service.Login(ApiKey, SetLoggedIn);
		}

		private void SetLoggedIn(bool successfullyLoggedIn)
		{
			if (successfullyLoggedIn)
				SaveApiKey();

			isLoggedIn = successfullyLoggedIn;
			UpdateErrorMessage();
			RaisePropertyChanged("LoginPanelVisibility");
			RaisePropertyChanged("EditorPanelVisibility");
			RaisePropertyChanged("IsLoggedIn");
		}

		public ICommand OnGetApiKeyClicked { get; private set; }
		public ICommand OnLogoutButtonClicked { get; private set; }

		private void Logout()
		{
			ApiKey = "";
			SaveApiKey();
			SetLoggedIn(false);
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
				try
				{
					var instance = Activator.CreateInstance(pluginType, Service) as EditorPluginView;
					if (instance != null)
						InsertPluginAtRightPriority(instance);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
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