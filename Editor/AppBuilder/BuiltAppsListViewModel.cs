using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
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
			DeleteObsoleteAppsStorageXmlFileIfExists();
			FindBuiltApps();
		}

		private readonly List<AppInfo> builtAppsList;
		public string AppStorageDirectory { get; private set; }

		private void DeleteObsoleteAppsStorageXmlFileIfExists()
		{
			// This functionality can be removed in some later releases after 0.9.9.3 again
			string xmlStorageFilePath = Path.Combine(AppStorageDirectory, "AppsStorage.xml");
			if (File.Exists(xmlStorageFilePath))
				TryDeleteFile(xmlStorageFilePath);
		}

		private static void TryDeleteFile(string filePath)
		{
			try
			{
				File.Delete(filePath);
			}
			catch (Exception ex)
			{
				Logger.Warning("Unable to delete '" + filePath + "' file because: " + ex);
			}
		}

		private void FindBuiltApps()
		{
			Logger.Info(AppStorageDirectory);
			if (!Directory.Exists(AppStorageDirectory))
				return;
			var files = Directory.GetFiles(AppStorageDirectory);
			foreach (var filePath in files)
				if (!filePath.EndsWith(".db"))
					LoadBuiltAppToList(filePath);
		}

		private void LoadBuiltAppToList(string appPackageFilePath)
		{
			try
			{
				AppInfo app = AppInfoExtensions.CreateAppInfo(appPackageFilePath,
					PlatformNameExtensions.GetPlatformFromFileExtension(Path.GetExtension(appPackageFilePath)),
					GetDummyAppGuid(Path.GetFileNameWithoutExtension(appPackageFilePath)),
					File.GetCreationTime(appPackageFilePath));
				builtAppsList.Add(app);
			}
			catch (Exception ex)
			{
				Logger.Warning("Unable to load already built app '" + appPackageFilePath + "':" +
					Environment.NewLine + ex);
			}
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
			string appSavePath = Path.Combine(AppStorageDirectory, Path.GetFileName(appInfo.FilePath));
			File.WriteAllBytes(appSavePath, appData);
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