using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
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
			viewModel = new AppBuilderViewModel(new MockBuildService());
		}

		private AppBuilderViewModel viewModel;

		[Test]
		public void CheckSupportedAndSelectedPlatform()
		{
			Assert.IsNotEmpty(viewModel.SupportedPlatforms);
			foreach (PlatformName platform in viewModel.SupportedPlatforms)
				Logger.Info("SupportedPlatform: " + platform);
			Assert.AreNotEqual(0, viewModel.SelectedPlatform);
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
				CheckAvailableProjectsOfSamplesSolution();
			}
			finally
			{
				Environment.CurrentDirectory = originalDirectory;
			}
		}

		private void CheckAvailableProjectsOfSamplesSolution()
		{
			Assert.IsNotEmpty(viewModel.AvailableProjectsInSelectedSolution);
			Assert.IsNotNull(viewModel.SelectedSolutionProject);
			Assert.Contains(viewModel.SelectedSolutionProject,
				viewModel.AvailableProjectsInSelectedSolution);
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
				CheckAvailableProjectsOfSamplesSolution();
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
		public void ExcuteBuild()
		{
			Assert.IsTrue(viewModel.BuildPressed.CanExecute(null));
			viewModel.BuiltAppRecieved += (app, data) => Logger.Info("Built app received: " + app.Name);
			viewModel.BuildPressed.Execute(null);
		}
	}
}