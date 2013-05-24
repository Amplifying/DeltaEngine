using System;
using System.IO;
using System.Threading;
using DeltaEngine.Editor.Common;
using DeltaEngine.Networking.Sockets;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class BuildServiceConnectionTests
	{
		[SetUp]
		public void StartBuilderServiceClient()
		{
			service = new BuildServiceConnectionViaLAN();
		}

		private BuildServiceConnectionViaLAN service;

		[TearDown]
		public void StopBuilderServiceClient()
		{
			service = null;
		}

		[Test, Category("Slow")]
		public void CheckConnectionStateOfService()
		{
			Assert.IsTrue(service.Client.IsConnected);
			Assert.IsTrue(service.IsLoggedIn);
		}

		private static void WaitForServerResponse(int milliseconds = 100)
		{
			Thread.Sleep(milliseconds);
		}

		[Test, Category("Slow")]
		public void SendBuildRequestToServer()
		{
			service.BuildMessageRecieved += LogBuildMessageToConsole;
			service.BuildResultRecieved += CheckForSuccessfulBuildResult;
			//service.SendMessage(new BuildRequest("LogoApp", PlatformName.WindowsPhone7, new byte[1]));

			string userSolutionPath = @"C:\Development\DeltaEngineServices\GeneratedCode\" +
				@"LogoApp.original\LogoApp.sln";
			string projectName = Path.GetFileNameWithoutExtension(userSolutionPath);
			var projectData = new CodeData(Path.GetDirectoryName(userSolutionPath));
			var request = new BuildRequest(projectName, PlatformName.WindowsPhone7, projectData.GetBytes())
			{
				SolutionFilePath = Path.GetFileName(userSolutionPath),
			};
			service.SendMessage(request);
			WaitForServerResponse(1000);
		}

		private static void LogBuildMessageToConsole(BuildMessage buildMessage)
		{
			Assert.AreNotEqual(BuildMessageType.BuildError, buildMessage.Type);
			Console.WriteLine(buildMessage);
		}

		private static void CheckForSuccessfulBuildResult(BuildResult buildResult)
		{
			Assert.IsNull(buildResult.BuildError);
			Assert.AreNotEqual(Guid.Empty, buildResult.PackageGuid);
			Assert.IsNotEmpty(buildResult.PackageFileData);
		}
	}
}