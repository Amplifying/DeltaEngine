using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppBuilderViewModelTests
	{
		[SetUp]
		public void LoadAppBuilderViewModel()
		{
			new MockLogger();
			CreateNewAppBuilderViewModel();
		}

		private void CreateNewAppBuilderViewModel()
		{
			service = new MockBuildService();
			viewModel = new AppBuilderViewModel(service);
		}

		private MockBuildService service;
		private AppBuilderViewModel viewModel;

		[Test]
		public void CheckSupportedAndSelectedPlatform()
		{
			Assert.IsNotEmpty(viewModel.SupportedPlatforms);
			foreach (PlatformName platform in viewModel.SupportedPlatforms)
				Logger.Info("SupportedPlatform: " + platform);
			Assert.AreNotEqual((PlatformName)0, viewModel.SelectedPlatform);
		}
		
		[Test]
		public void ThereShouldBeAlwaysAStartupSolution()
		{
			Assert.IsTrue(File.Exists(viewModel.UserSolutionPath), viewModel.UserSolutionPath);
		}

		[Test]
		public void CheckAvailableProjectsOfSamplesSolutionInEngineCodeDirectory()
		{
			string originalDirectory = Environment.CurrentDirectory;
			try
			{
				Environment.CurrentDirectory = PathExtensions.GetFallbackEngineSourceCodeDirectory();
				CreateNewAppBuilderViewModel();
				AssertSelectedSolutionProject();
			}
			finally
			{
				Environment.CurrentDirectory = originalDirectory;
			}
		}

		private void AssertSelectedSolutionProject()
		{
			Assert.AreEqual(viewModel.SelectedSolutionProject.Title, service.ProjectName);
		}

		[Test]
		public void CheckAvailableProjectsOfSamplesSolutionInEngineInstallerDirectory()
		{
			const string EnginePathVariableName = PathExtensions.EnginePathEnvironmentVariableName;
			string originalDirectory = Environment.GetEnvironmentVariable(EnginePathVariableName);
			try
			{
				Environment.SetEnvironmentVariable(EnginePathVariableName, null);
				CreateNewAppBuilderViewModel();
				AssertSelectedSolutionProject();
			}
			finally
			{
				Environment.SetEnvironmentVariable(EnginePathVariableName, originalDirectory);
			}
		}

		[Test]
		public void CheckAvailableEntryPoints()
		{
			Assert.IsNotEmpty(viewModel.AvailableEntryPointsInSelectedProject);
			Assert.IsNotEmpty(viewModel.SelectedEntryPoint);
			Assert.Contains(viewModel.SelectedEntryPoint,
				viewModel.AvailableEntryPointsInSelectedProject);
		}

		[Test]
		public void SelectedSolutionProjectMustUpdateWhenProjectNameHasChanged()
		{
			AssertSelectedProjectIsBuildable();
			service.ChangeProject("LogoApp");
			AssertSelectedProjectIsBuildable();
			service.ChangeProject("GhostWars");
			AssertSelectedProjectIsBuildable();
		}

		private void AssertSelectedProjectIsBuildable()
		{
			AssertSelectedSolutionProject();
			Assert.IsTrue(viewModel.IsBuildActionExecutable);
		}

		[Test]
		public void NonExistingProjectCanNotBeBuild()
		{
			AssertSelectedProjectIsBuildable();
			service.ChangeProject("NonExistingProject");
			Assert.IsNull(viewModel.SelectedSolutionProject);
			Assert.IsFalse(viewModel.IsBuildActionExecutable);
		}

		[Test]
		public void ExcuteBuildCommand()
		{
			Assert.IsTrue(viewModel.IsBuildActionExecutable);
			Assert.IsTrue(viewModel.BuildCommand.CanExecute(null));
			viewModel.BuiltAppRecieved += (app, data) => Logger.Info("Built app received: " + app.Name);
			viewModel.BuildCommand.Execute(null);
		}

		[Test, Category("Slow"), Timeout(10000)]
		public void RequestRequild()
		{
			int numberOfRequests = service.NumberOfBuildRequests;
			AppInfo app = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", PlatformName.Windows);
			app.SolutionFilePath = AppBuilderViewModel.GetSamplesSolutionFilePath();
			viewModel.AppListViewModel.RequestRebuild(app);
			Assert.AreEqual(numberOfRequests + 1, service.NumberOfBuildRequests);
		}

		[Test]
		public void AppBuilderShouldBeAbleToHandleBuildMessage()
		{
			int numberOfWarngins = viewModel.MessagesListViewModel.Warnings.Count;
			service.RaiseAppBuildInfo("You shouldn't see this message");
			service.RaiseAppBuildWarning("An other build warning");
			Assert.AreEqual(numberOfWarngins + 1, viewModel.MessagesListViewModel.Warnings.Count);
		}

		[Test]
		public void AppBuilderShouldBeAbleToHandleFailedBuild()
		{
			Assert.IsTrue(viewModel.IsBuildActionExecutable);
			viewModel.AppBuildFailedRecieved += (fail) => Logger.Info("Built failed: " + fail.Reason);
			service.ReceiveAppBuildFailed("Info messages won't be collected");
			Assert.IsTrue(viewModel.IsBuildActionExecutable);
		}

		// ncrunch: no coverage start
		[Test, Category("Slow")]
		public void ExcuteHelpCommand()
		{
			Assert.IsTrue(viewModel.HelpCommand.CanExecute(null));
			viewModel.HelpCommand.Execute(null);
		}
	}
}