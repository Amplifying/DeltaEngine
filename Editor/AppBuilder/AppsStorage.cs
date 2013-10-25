using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.AppBuilder
{
	public class AppsStorage
	{
		public AppsStorage(Settings settings)
		{
			this.settings = settings;
			availableApps = new List<AppInfo>();
			LoadAlreadyBuiltApps();
		}

		private readonly Settings settings;
		private readonly List<AppInfo> availableApps;

		private void LoadAlreadyBuiltApps()
		{
			string storageDataAsXml = settings.GetValue(XmlNodeNameOfStorageData, "");
			if (String.IsNullOrEmpty(storageDataAsXml))
				CreateNewBuiltAppsListData();
			else
				storageData = new XmlSnippet(storageDataAsXml).Root;
			StorageDirectory = storageData.GetAttributeValue("StoragePath");
			foreach (XmlData appInfoData in storageData.Children)
				TryLoadAppFromStorageData(appInfoData);
		}

		private const string XmlNodeNameOfStorageData = "BuiltApps";
		private XmlData storageData;

		private void CreateNewBuiltAppsListData()
		{
			storageData = new XmlData(XmlNodeNameOfStorageData);
			storageData.AddAttribute("StoragePath",
				Path.Combine(AssemblyExtensions.GetMyDocumentsAppFolder(), storageData.Name));
			UpdateStorageDataInSettings();
		}

		private void UpdateStorageDataInSettings()
		{
			settings.SetValue(storageData.Name, storageData.ToXmlString());
		}

		public string StorageDirectory { get; private set; }

		private void TryLoadAppFromStorageData(XmlData appInfoData)
		{
			try
			{
				LoadAppFromStorageData(appInfoData);
			}
			// ncrunch: no coverage start
			catch (Exception ex)
			{
				Logger.Warning("Unable to load '" + appInfoData.ToXmlString() + "' as app :" +
					Environment.NewLine + ex);
			}
			// ncrunch: no coverage end
		}

		private void LoadAppFromStorageData(XmlData appInfoData)
		{
			AppInfo app = AppInfoExtensions.CreateAppInfo(GetAppPackageFilePath(appInfoData),
				GetAppPlatform(appInfoData), GetAppGuid(appInfoData), GetAppBuildData(appInfoData));
			app.SolutionFilePath = GetAppSolutionFilePath(appInfoData);
			availableApps.Add(app);
		}

		private string GetAppPackageFilePath(XmlData appInfoData)
		{
			string fileName = appInfoData.GetAttributeValue(XmlAttributeNameOfFileName);
			if (String.IsNullOrEmpty(fileName))
				throw new AppInfoDataMissing(XmlAttributeNameOfFileName);
			return Path.Combine(StorageDirectory, fileName);
		}

		private const string XmlAttributeNameOfFileName = "FileName";

		private class AppInfoDataMissing : Exception
		{
			public AppInfoDataMissing(string xmlAttributeName)
				: base(xmlAttributeName) { }
		}

		private static PlatformName GetAppPlatform(XmlData appInfoData)
		{
			const PlatformName NoPlatform = (PlatformName)0;
			PlatformName platform = appInfoData.GetAttributeValue(XmlAttributeNameOfPlatform, NoPlatform);
			if (platform == NoPlatform)
				throw new AppInfoDataMissing(XmlAttributeNameOfPlatform);
			return platform;
		}

		private const string XmlAttributeNameOfPlatform = "Platform";

		private static Guid GetAppGuid(XmlData appInfoData)
		{
			string appGuidString = appInfoData.GetAttributeValue(XmlAttributeNameOfAppGuid);
			if (String.IsNullOrEmpty(appGuidString))
				throw new AppInfoDataMissing(XmlAttributeNameOfAppGuid);
			return new Guid(appGuidString);
		}

		private const string XmlAttributeNameOfAppGuid = "AppGuid";

		private static DateTime GetAppBuildData(XmlData appInfoData)
		{
			var noBuildDate = DateTime.MinValue;
			DateTime buildDate = appInfoData.GetAttributeValue(XmlAttributeNameOfBuildDate, noBuildDate);
			if (buildDate == noBuildDate)
				throw new AppInfoDataMissing(XmlAttributeNameOfBuildDate);
			return buildDate;
		}

		private const string XmlAttributeNameOfBuildDate = "BuildDate";

		private static string GetAppSolutionFilePath(XmlData appInfoData)
		{
			string solutionFilePath = appInfoData.GetAttributeValue(XmlAttributeNameOfSolutionFilePath);
			if (String.IsNullOrEmpty(solutionFilePath))
				throw new AppInfoDataMissing(XmlAttributeNameOfSolutionFilePath);
			return solutionFilePath;
		}

		private const string XmlAttributeNameOfSolutionFilePath = "SolutionFilePath";

		public AppInfo[] AvailableApps
		{
			get { return availableApps.ToArray(); }
		}

		public void AddApp(AppInfo app)
		{
			int indexOfApp = availableApps.FindIndex(a => a.Name == app.Name);
			if (indexOfApp != InvalidIndex)
			{
				availableApps[indexOfApp] = app;
				UpdateStorageDataInSettings();
			}
			else
			{
				availableApps.Add(app);
				AddAppToStorageData(app);
			}
		}

		private const int InvalidIndex = -1;

		private void AddAppToStorageData(AppInfo app)
		{
			var appDataNode = new XmlData("App");
			appDataNode.AddAttribute(XmlAttributeNameOfFileName, Path.GetFileName(app.FilePath));
			appDataNode.AddAttribute(XmlAttributeNameOfPlatform, app.Platform);
			appDataNode.AddAttribute(XmlAttributeNameOfAppGuid, app.AppGuid);
			appDataNode.AddAttribute(XmlAttributeNameOfBuildDate, app.BuildDate);
			appDataNode.AddAttribute(XmlAttributeNameOfSolutionFilePath, app.SolutionFilePath);
			storageData.AddChild(appDataNode);
			UpdateStorageDataInSettings();
		}
	}
}