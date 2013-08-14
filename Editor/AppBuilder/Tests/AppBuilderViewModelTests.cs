using System;
using System.IO;
using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppBuilderViewModelTests
	{
		[Test]
		public void CheckSupportedAndSelectedPlatform()
		{
			var viewModel = GetViewModelWithMockService();
			Assert.IsNotEmpty(viewModel.SupportedPlatforms);
			foreach (PlatformName platform in viewModel.SupportedPlatforms)
				Console.WriteLine(platform);
			Assert.AreNotEqual(0, viewModel.SelectedPlatform);
		}

		private static AppBuilderViewModel GetViewModelWithMockService()
		{
			return new AppBuilderViewModel(new MockBuildService());
		}

		[Test]
		public void ExcuteBrowseUserSolutionPath()
		{
			var viewModel = GetViewModelWithMockServiceAndSamplesSelection();
			Assert.IsTrue(viewModel.UserSolutionPath.EndsWith("DeltaEngine.Samples.sln"));
			Assert.IsTrue(File.Exists(viewModel.UserSolutionPath));
		}

		private static AppBuilderViewModel GetViewModelWithMockServiceAndSamplesSelection()
		{
			var viewModel = GetViewModelWithMockService();
			Assert.IsTrue(viewModel.BrowsePressed.CanExecute(null));
			string samplesSolution = PathExtensions.GetSamplesSolutionFilePath();
			viewModel.BrowsePressed.Execute(samplesSolution);
			return viewModel;
		}

		[Test]
		public void CheckAvailableProjectsOfSelectedSolution()
		{
			var viewModel = GetViewModelWithMockServiceAndSamplesSelection();
			Assert.IsNotEmpty(viewModel.AvailableProjectsInSelectedSolution);
			Assert.IsNotNull(viewModel.SelectedSolutionProject);
			Console.WriteLine("SelectedSolutionProject: " + viewModel.SelectedSolutionProject.Title);
			Assert.IsTrue(
				viewModel.AvailableProjectsInSelectedSolution.Contains(viewModel.SelectedSolutionProject));
		}

		[Test]
		public void CheckAvailableEntryPoints()
		{
			var viewModel = GetViewModelWithMockServiceAndSamplesSelection();
			Assert.IsNotEmpty(viewModel.AvailableEntryPointsInSelectedProject);
			Assert.IsNotEmpty(viewModel.SelectedEntryPoint);
			Console.WriteLine("SelectedEntryPoint: " + viewModel.SelectedEntryPoint);
			Assert.IsTrue(
				viewModel.AvailableEntryPointsInSelectedProject.Contains(viewModel.SelectedEntryPoint));
		}

		[Test]
		public void ExcuteBuild()
		{
			var viewModel = GetViewModelWithMockServiceAndSamplesSelection();
			Assert.IsTrue(viewModel.BuildPressed.CanExecute(null));
			viewModel.BuiltAppRecieved += (app, data) => Console.WriteLine(app.Name);
			viewModel.BuildPressed.Execute(null);
		}
	}
}