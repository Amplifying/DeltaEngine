using System;
using DeltaEngine.Editor.Common;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class BuilderViewModelTests
	{
		[Test]
		public void Start()
		{
			var viewModel = new MockBuilderViewModel();
			Assert.IsNotEmpty(viewModel.SupportedPlatforms);
			foreach (PlatformName platform in viewModel.SupportedPlatforms)
				Console.WriteLine(platform);
			Assert.AreNotEqual(0, viewModel.UserSelectedPlatform);
			Assert.IsTrue(viewModel.UserProjectPath.EndsWith(".sln"));
			Assert.IsNotEmpty(viewModel.UserProjectEntryPoints);
			Assert.IsNotEmpty(viewModel.UserSelectedEntryPoint);
		}

		[Test]
		public void ExcuteBuild()
		{
			var viewModel = new MockBuilderViewModel();
			Assert.IsTrue(viewModel.BuildPressed.CanExecute(null));
			viewModel.BuildPressed.Execute(null);
		}
	}
}