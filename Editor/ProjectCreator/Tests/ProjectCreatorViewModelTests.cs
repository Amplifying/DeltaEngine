using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the logic of the project creation editor plugin.
	/// </summary>
	public class ProjectCreatorViewModelTests
	{
		[SetUp]
		public void Create()
		{
			viewModel = new ProjectCreatorViewModel();
		}

		private ProjectCreatorViewModel viewModel;

		[Test]
		public void ChangeName()
		{
			const string NewName = "ChangedProjectName";
			viewModel.OnNameChanged.Execute(NewName);
			Assert.AreEqual(NewName, viewModel.Name);
		}

		//TODO: fix
		[Test, Ignore]
		public void CheckAvailableFrameworks()
		{
			Assert.AreEqual(DeltaEngineFramework.GLFW, viewModel.AvailableFrameworks[0]);
			Assert.AreEqual(DeltaEngineFramework.MonoGame, viewModel.AvailableFrameworks[1]);
			Assert.AreEqual(DeltaEngineFramework.OpenTK, viewModel.AvailableFrameworks[2]);
			Assert.AreEqual(DeltaEngineFramework.SharpDX, viewModel.AvailableFrameworks[3]);
			Assert.AreEqual(DeltaEngineFramework.SlimDX, viewModel.AvailableFrameworks[4]);
			Assert.AreEqual(DeltaEngineFramework.Xna, viewModel.AvailableFrameworks[5]);
		}

		/*TODO:
		[TestCase(0, DeltaEngineFramework.GLFW)]
		[TestCase(1, DeltaEngineFramework.MonoGame)]
		[TestCase(2, DeltaEngineFramework.OpenTK)]
		[TestCase(3, DeltaEngineFramework.SharpDX)]
		[TestCase(4, DeltaEngineFramework.SlimDX)]
		[TestCase(5, DeltaEngineFramework.Xna)]
		public void ChangeSelection(int id, DeltaEngineFramework expectedFramework)
		{
			viewModel.OnFrameworkSelectionChanged.Execute(id);
			Assert.AreEqual(expectedFramework, viewModel.SelectedFramework);
		}
		 */

		[Test]
		public void ChangePath()
		{
			const string NewPath = "C:\\DeltaEngine\\";
			viewModel.OnLocationChanged.Execute(NewPath);
			Assert.AreEqual(NewPath, viewModel.Location);
		}

		[Test]
		public void CanCreateProjectWithValidName()
		{
			viewModel.OnNameChanged.Execute("ValidProjectName");
			Assert.IsTrue(viewModel.OnCreateClicked.CanExecute(null));
		}

		[Test]
		public void CannotCreateProjectWithInvalidName()
		{
			viewModel.OnNameChanged.Execute("Invalid Project Name");
			Assert.IsFalse(viewModel.OnCreateClicked.CanExecute(null));
		}

		[Test]
		public void CanCreateProjectWithValidLocation()
		{
			viewModel.OnLocationChanged.Execute("C:\\ValidLocation\\");
			Assert.IsTrue(viewModel.OnCreateClicked.CanExecute(null));
		}

		[Test]
		public void CannotCreateProjectWithInvalidLocation()
		{
			viewModel.OnLocationChanged.Execute("Invalid Location");
			Assert.IsFalse(viewModel.OnCreateClicked.CanExecute(null));
		}
	}
}