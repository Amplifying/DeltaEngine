using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// The ViewModel for BuiltAppsListView which manages all existing apps that were built via the
	/// BuildService.
	/// </summary>
	public class BuiltAppsListViewModel : ViewModelBase
	{
		public BuiltAppsListViewModel()
		{
			builtAppsList = new List<AppInfo>();
			AppStorageDirectory = Path.Combine(AssemblyExtensions.GetMyDocumentsAppFolder(), "BuiltApps");
			appStorageData = new XmlData("AppsStorage");
		}

		private readonly List<AppInfo> builtAppsList;
		public string AppStorageDirectory { get; set; }
		private XmlData appStorageData;

		public AppInfo[] BuiltApps
		{
			get { return builtAppsList.ToArray(); }
		}

		public int NumberOfBuiltApps
		{
			get { return builtAppsList.Count; }
		}

		public void AddApp(AppInfo appInfo, byte[] appData = null)
		{
			if (appInfo == null)
				throw new NoAppInfoSpecified();
			builtAppsList.Add(appInfo);
			RaisePropertyChangedOfBuiltApps();
			if (appData != null)
				SaveBuiltApp(appInfo, appData);
		}

		public class NoAppInfoSpecified : Exception {}

		private void RaisePropertyChangedOfBuiltApps()
		{
			RaisePropertyChanged("BuiltApps");
			RaisePropertyChanged("TextOfBuiltApps");
		}

		private void SaveBuiltApp(AppInfo appInfo, byte[] appData)
		{
			ValidateAppStorageDirectory();
			CreateAppStorageDirectoryIfNecessary();
			File.WriteAllBytes(appInfo.FilePath, appData);
			var appMetaData = new XmlData("App");
			appMetaData.AddAttribute(AttributeNamePackageFileName, Path.GetFileName(appInfo.FilePath));
			appMetaData.AddAttribute(AttributeNamePlatform, appInfo.Platform);
			appMetaData.AddAttribute(AttributeNamePackageGuid, appInfo.AppGuid);
			if (appInfo.IsSolutionPathAvailable)
				appMetaData.AddAttribute(AttributeNameSolutionPath, appInfo.SolutionFilePath);
			appStorageData.AddChild(appMetaData);
			File.WriteAllText(GetAppsStorageDataFilePath(), appStorageData.ToXmlString());
		}

		private void ValidateAppStorageDirectory()
		{
			if (String.IsNullOrEmpty(AppStorageDirectory))
				throw new NoAppStorageDirectorySpecified();
		}

		public class NoAppStorageDirectorySpecified : Exception {}

		private void CreateAppStorageDirectoryIfNecessary()
		{
			if (!Directory.Exists(AppStorageDirectory))
				Directory.CreateDirectory(AppStorageDirectory);
		}

		private const string AttributeNamePackageFileName = "PackageFileName";
		private const string AttributeNamePlatform = "Platform";
		private const string AttributeNamePackageGuid = "AppGuid";
		private const string AttributeNameSolutionPath = "SolutionPath";

		private string GetAppsStorageDataFilePath()
		{
			return Path.Combine(AppStorageDirectory, "AppsStorage.xml");
		}

		public void Load()
		{
			ValidateAppStorageDirectory();
			string storageDataFilePath = GetAppsStorageDataFilePath();
			if (File.Exists(storageDataFilePath))
				appStorageData = new XmlFile(storageDataFilePath).Root;
			foreach (XmlData appEntry in appStorageData.Children)
				LoadSavedApp(appEntry);
			RaisePropertyChangedOfBuiltApps();
		}

		private void LoadSavedApp(XmlData appMetaData)
		{
			string packageFilePath = Path.Combine(AppStorageDirectory,
				appMetaData.GetAttributeValue(AttributeNamePackageFileName));
			var appPlatform = appMetaData.GetAttributeValue(AttributeNamePlatform).Parse<PlatformName>();
			var appGuid = new Guid(appMetaData.GetAttributeValue(AttributeNamePackageGuid));
			var loadedAppInfo = AppInfoExtensions.CreateAppInfo(packageFilePath, appPlatform, appGuid);
			loadedAppInfo.SolutionFilePath = appMetaData.GetAttributeValue(AttributeNameSolutionPath);
			builtAppsList.Add(loadedAppInfo);
		}

		public string TextOfBuiltApps
		{
			get { return "App".GetCountAndWordInPluralIfNeeded(builtAppsList.Count); }
		}

		public void RequestRebuild(AppInfo app)
		{
			if (RebuildRequest != null)
				RebuildRequest(app);
		}

		public event Action<AppInfo> RebuildRequest;
	}

	internal class DemoBuiltAppsListForDesigner : BuiltAppsListViewModel
	{
		public DemoBuiltAppsListForDesigner()
		{
			AddApp(new WindowsAppInfo("Rebuildable app", Guid.NewGuid()) { SolutionFilePath = "A.sln" });
			AddApp(new WindowsAppInfo("Non-Rebuildable app ", Guid.NewGuid()));
		}
	}
}