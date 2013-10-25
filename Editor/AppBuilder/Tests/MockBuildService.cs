using System;
using System.Threading;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Editor.Mocks;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class MockBuildService : MockService
	{
		public MockBuildService()
			: base(typeof(MockBuildService).Name + "User", "LogoApp")
		{
			Platforms = new[] { PlatformName.Windows };
			resultBuilder = new MockAppResultBuilder(this);
			NumberOfBuildRequests = 0;
		}

		public PlatformName[] Platforms { get; private set; }
		private readonly MockAppResultBuilder resultBuilder;
		public int NumberOfBuildRequests { get; private set; }

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

		public override void Send(object message, bool allowToCompressMessage = true)
		{
			if (message is AppBuildRequest)
				OnHandleBuildRequest((AppBuildRequest)message);
			if (message is SupportedPlatformsRequest)
				ReceiveData(new SupportedPlatformsResult(SupportedPlatforms));
		}

		private static readonly PlatformName[] SupportedPlatforms = new[]
			{ PlatformName.Windows, PlatformName.Android, PlatformName.Web, };

		private void OnHandleBuildRequest(AppBuildRequest userBuildRequest)
		{
			NumberOfBuildRequests++;
			resultBuilder.BuildApp(userBuildRequest);
		}

		public void ReceiveSomeTestMessages()
		{
			RaiseAppBuildWarning("A BuildWarning");
			ReceiveData(AppBuilderTestExtensions.AsBuildTestError("A BuildError"));
		}

		public void RaiseAppBuildInfo(string message)
		{
			ReceiveData(AppBuilderTestExtensions.AsBuildTestInfo(message));
		}

		public void RaiseAppBuildWarning(string message)
		{
			ReceiveData(AppBuilderTestExtensions.AsBuildTestWarning(message));
		}

		public void ReceiveAppBuildFailed(string reason)
		{
			ReceiveData(new AppBuildFailed(reason));
		}
	}
}