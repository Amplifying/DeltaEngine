using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Editor.Messages;
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
			FindBuiltApps();
		}

		private readonly List<AppInfo> builtAppsList;
		public string AppStorageDirectory { get; set; }
		private XmlData appStorageData;

		private void FindBuiltApps()
		{
			if (!Directory.Exists(AppStorageDirectory))
				return;
			var files = Directory.GetFiles(AppStorageDirectory);
			foreach (var filePath in files)
				if (!filePath.EndsWith(".db"))
					builtAppsList.Add(AppInfoExtensions.CreateAppInfo(filePath,
						PlatformNameExtensions.GetPlatformFromFileExtension(Path.GetExtension(filePath)),
						GetDummyAppGuid(Path.GetFileNameWithoutExtension(filePath)),
						File.GetCreationTime(filePath)));
		}

		private static Guid GetDummyAppGuid(string appName)
		{
			//loading it from the DeltaEngine.Samples.sln is not that hard, do not do this manually!
			appName = appName.ToLower();
			if (appName.StartsWith("logoapp"))
				return new Guid("4d33a50e-3aa2-4e7e-bc0c-4ef7b3d5e985");
			if (appName.StartsWith("breakout"))
				return new Guid("2e78abad-fe79-455a-8393-48e08de57064");
			if (appName.StartsWith("blocks"))
				return new Guid("8d02900e-a9a6-4510-acd1-f8df74602ed0");
			if (appName.StartsWith("ghostwars"))
				return new Guid("039f9138-e9f4-4abe-899c-18cfadd7b930");
			return Guid.Empty;
		}

		public AppInfo[] BuiltApps
		{
			//TODO: filter by selected platform and only return those packages
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

		//never called except in tests
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
			var appPlatform =
				appMetaData.GetAttributeValue(AttributeNamePlatform).TryParse(PlatformName.Windows);
			var appGuid = new Guid(appMetaData.GetAttributeValue(AttributeNamePackageGuid));
			var loadedAppInfo = AppInfoExtensions.CreateAppInfo(packageFilePath, appPlatform, appGuid,
				DateTime.Now);
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
}