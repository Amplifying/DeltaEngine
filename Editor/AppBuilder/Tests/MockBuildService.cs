using System;
using System.Collections.Generic;
using System.Threading;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class MockBuildService : Service
	{
		public MockBuildService()
		{
			UserName = GetType().Name + "User";
			ProjectName = "LogoApp";
			Platforms = new[] { PlatformName.Windows };
			resultBuilder = new MockAppResultBuilder(this);
		}

		public string UserName { get; private set; }
		public string ProjectName { get; private set; }
		public PlatformName[] Platforms { get; private set; }
		private readonly MockAppResultBuilder resultBuilder;

		private class MockAppResultBuilder
		{
			public MockAppResultBuilder(MockBuildService service)
			{
				this.service = service;
			}

			private readonly MockBuildService service;

			public void BuildApp(AppBuildRequest userBuildRequest)
			{
				buildRequest = userBuildRequest;
				waitingTimeInMs = 0;
				if (IsWaitTimeRequired(userBuildRequest.ProjectName))
					waitingTimeInMs = 1000;
				worker = new Thread(Run);
				worker.Start();
			}

			private AppBuildRequest buildRequest;
			private int waitingTimeInMs;

			private static bool IsWaitTimeRequired(string projectName)
			{
				return projectName != "LogoApp" || projectName == "DeltaEngine";
			}

			private Thread worker;

			private void Run()
			{
				int startTime = Environment.TickCount;
				while ((Environment.TickCount - startTime) < waitingTimeInMs)
					Thread.Sleep(0);
				service.ReceiveData(GetBuiltResult());
			}

			private object GetBuiltResult()
			{
				if (buildRequest.ProjectName == "DeltaEngine")
					return new AppBuildFailed("Demo message for failed build of a project");
				return new AppBuildResult(buildRequest.ProjectName, buildRequest.Platform)
				{
					PackageFileName = buildRequest.ProjectName + ".app",
				};
			}
		}

		public event Action<object> DataReceived;
		public event Action ContentChanged;

		public void Send(object message)
		{
			if (message is AppBuildRequest)
				OnHandleBuildRequest((AppBuildRequest)message);
		}

		private void OnHandleBuildRequest(AppBuildRequest userBuildRequest)
		{
			resultBuilder.BuildApp(userBuildRequest);
		}

		public void ReceiveData(object fakedReceivedData)
		{
			DataReceived(fakedReceivedData);
		}

		public void ChangeContent()
		{
			ContentChanged();
		}

		public void ReceiveSomeTestMessages()
		{
			ReceiveData("A BuildWarning".AsBuildTestWarning());
			ReceiveData("A BuildError".AsBuildTestError());
		}

		public IEnumerable<string> GetAllContentNames()
		{
			return new List<string>();
		}

		public IEnumerable<string> GetAllContentNamesByType(ContentType type)
		{
			return new List<string>();
		}

		public ContentType? GetTypeOfContent(string content)
		{
			return ContentType.Image;
		}

		public void UploadContent(ContentMetaData metaData,
			Dictionary<string, byte[]> optionalFileData = null) {}

		public void DeleteContent(string selectedContent, bool deleteSubContent = false) {}


	}
}