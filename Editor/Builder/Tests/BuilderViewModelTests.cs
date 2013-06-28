using System;
using System.IO;
using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class BuilderViewModelTests
	{
		[Test]
		public void CheckSupportedAndSelectedPlatform()
		{
			var viewModel = GetViewModelWithMockService();
			Assert.IsNotEmpty(viewModel.SupportedPlatforms);
			foreach (PlatformName platform in viewModel.SupportedPlatforms)
				Console.WriteLine(platform);
			Assert.AreNotEqual(0, viewModel.UserSelectedPlatform);
		}

		private static BuilderViewModel GetViewModelWithMockService()
		{
			var viewModel = new BuilderViewModel(new BuilderMockService());
			viewModel.SelectSamplesSolution();
			return viewModel;
		}

		[Test]
		public void CheckDefaultSelectedCodeSolutionAndAvailableProjects()
		{
			var viewModel = GetViewModelWithMockService();
			Assert.IsTrue(viewModel.UserSolutionPath.EndsWith("DeltaEngine.Samples.sln"));
			Assert.IsTrue(File.Exists(viewModel.UserSolutionPath));
		}

		[Test]
		public void CheckAvailableProjectsOfSelectedSolution()
		{
			var viewModel = GetViewModelWithMockService();
			Assert.IsNotEmpty(viewModel.AvailableProjectsInSelectedSolution);
			Assert.IsNotNull(viewModel.SelectedProject);
			Console.WriteLine("SelectedProject: " + viewModel.SelectedProject.Title);
			Assert.IsTrue(
				viewModel.AvailableProjectsInSelectedSolution.Contains(viewModel.SelectedProject));
		}

		[Test]
		public void CheckAvailableEntryPoints()
		{
			var viewModel = GetViewModelWithMockService();
			Assert.IsNotEmpty(viewModel.AvailableEntryPointsInSelectedProject);
			Assert.IsNotEmpty(viewModel.SelectedEntryPoint);
			Console.WriteLine("SelectedEntryPoint: " + viewModel.SelectedEntryPoint);
			Assert.IsTrue(
				viewModel.AvailableEntryPointsInSelectedProject.Contains(viewModel.SelectedEntryPoint));
		}

		[Test]
		public void ExcuteBuild()
		{
			var viewModel = GetViewModelWithMockService();
			Assert.IsTrue(viewModel.BuildPressed.CanExecute(null));
			viewModel.BuildPressed.Execute(null);
		}
	}
}