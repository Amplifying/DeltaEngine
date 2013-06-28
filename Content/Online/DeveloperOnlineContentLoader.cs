using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Networking.Sockets;
using Microsoft.Win32;

namespace DeltaEngine.Content.Online
{
	public class DeveloperOnlineContentLoader : FileContentLoader
	{
		//ncrunch: no coverage start
		public DeveloperOnlineContentLoader(ContentDataResolver resolver)
			: base(resolver, "Content") {}

		protected override void LazyInitialize()
		{
			if (isInitialized)
				return;

			if (Directory.Exists(ContentPath))
			{
				base.LazyInitialize();
				return;
			}

			Directory.CreateDirectory(ContentPath);
			var apiKey = GetDeveloperApiKeyFromRegistry();
			if (String.IsNullOrEmpty(apiKey))
				throw new CannotDownloadDeveloperContentWithoutApiKeyPleaseStartEditor();
			var projectName = AssemblyExtensions.GetEntryAssemblyForProjectName();
			var connection = new TcpSocket();
			connection.Connect("deltaengine.net", 800);
			int timeout = 3000;
			while (!connection.IsConnected)
			{
				Thread.Sleep(10);
				timeout -= 10;
				if (timeout < 0)
					throw new UnableToConnectToContentService(connection.TargetAddress);
			}
			bool waitingForFiles = true;
			connection.DataReceived += receivedObject =>
			{
				var receivedFiles = receivedObject as ContentFiles;
				if (receivedFiles == null)
					return;
				receivedFiles.SaveAll(ContentPath);
				waitingForFiles = false;
			};
			connection.Send(new RequestContent(apiKey, projectName));
			timeout = 7000;
			while (waitingForFiles && connection.IsConnected)
			{
				Thread.Sleep(10);
				timeout -= 10;
				if (timeout < 0)
					throw new DidNotReceiveContentFilesOrApiKeyInvalid(projectName);
			}
			if (!connection.IsConnected)
				throw new ContentServerDisconnectedUs();

			base.LazyInitialize();
		}

		public class ContentServerDisconnectedUs : Exception {}

		public class CannotDownloadDeveloperContentWithoutApiKeyPleaseStartEditor : Exception {}

		private static string GetDeveloperApiKeyFromRegistry()
		{
			var registryKey = Registry.CurrentUser.OpenSubKey(RegistryPathForApiKey, false);
			if (registryKey != null)
				return (string)registryKey.GetValue("ApiKey");
			return "";
		}

		private const string RegistryPathForApiKey = @"Software\DeltaEngine\Editor";

		public class DidNotReceiveContentFilesOrApiKeyInvalid : Exception
		{
			public DidNotReceiveContentFilesOrApiKeyInvalid(string projectName)
				: base(projectName) {}
		}
		
		public class UnableToConnectToContentService : Exception
		{
			public UnableToConnectToContentService(string targetAddress)
				: base(targetAddress) {}
		}
	}

	public class ContentFiles
	{
		public void Add(string filename, byte[] fileBytes)
		{
			filenames.Add(filename);
			fileDatas.Add(fileBytes);
		}

		private readonly List<string> filenames = new List<string>();
		private readonly List<byte[]> fileDatas = new List<byte[]>();

		public void SaveAll(string contentPath)
		{
			if (filenames.Count == 0)
				throw new NoContentFilesReceived();
			for (int num = 0; num < filenames.Count; num++)
				File.WriteAllBytes(Path.Combine(contentPath, filenames[num]), fileDatas[num]);
		}

		public class NoContentFilesReceived : Exception {}
	}

	public class RequestContent
	{
		public RequestContent() {}

		public RequestContent(string apiKey, string projectName)
		{
			ApiKey = apiKey;
			ProjectName = projectName;
		}

		public string ApiKey { get; private set; }
		public string ProjectName { get; private set; }
	}
}