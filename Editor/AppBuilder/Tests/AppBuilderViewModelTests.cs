using System;
using System.IO;
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
			viewModel = new AppBuilderViewModel(new MockBuildService());
		}

		private AppBuilderViewModel viewModel;

		[Test]
		public void CheckSupportedAndSelectedPlatform()
		{
			Assert.IsNotEmpty(viewModel.SupportedPlatforms);
			foreach (PlatformName platform in viewModel.SupportedPlatforms)
				Console.WriteLine(platform);
			Assert.AreNotEqual(0, viewModel.SelectedPlatform);
		}
		
		[Test]
		public void ThereShouldBeAlwaysAStartupSolution()
		{
			Console.WriteLine(viewModel.UserSolutionPath);
			Assert.IsTrue(File.Exists(viewModel.UserSolutionPath));
		}

		[Test]
		public void CheckAvailableProjectsOfSelectedSolution()
		{
			Assert.IsNotEmpty(viewModel.AvailableProjectsInSelectedSolution);
			Assert.IsNotNull(viewModel.SelectedSolutionProject);
			Console.WriteLine("SelectedSolutionProject: " + viewModel.SelectedSolutionProject.Title);
			Assert.IsTrue(
				viewModel.AvailableProjectsInSelectedSolution.Contains(viewModel.SelectedSolutionProject));
		}

		[Test]
		public void CheckAvailableEntryPoints()
		{
			Assert.IsNotEmpty(viewModel.AvailableEntryPointsInSelectedProject);
			Assert.IsNotEmpty(viewModel.SelectedEntryPoint);
			Console.WriteLine("SelectedEntryPoint: " + viewModel.SelectedEntryPoint);
			Assert.IsTrue(
				viewModel.AvailableEntryPointsInSelectedProject.Contains(viewModel.SelectedEntryPoint));
		}

		[Test]
		public void ExcuteBuild()
		{
			Assert.IsTrue(viewModel.BuildPressed.CanExecute(null));
			viewModel.BuiltAppRecieved += (app, data) => Console.WriteLine(app.Name);
			viewModel.BuildPressed.Execute(null);
		}
	}
}