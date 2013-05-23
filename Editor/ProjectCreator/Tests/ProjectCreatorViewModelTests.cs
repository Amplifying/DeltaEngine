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

		[Test]
		public void CheckAvailableFrameworks()
		{
			Assert.AreEqual(4, viewModel.AvailableFrameworks.Count);
			Assert.AreEqual(DeltaEngineFramework.OpenTK, viewModel.AvailableFrameworks[0]);
			Assert.AreEqual(DeltaEngineFramework.SharpDX, viewModel.AvailableFrameworks[1]);
			Assert.AreEqual(DeltaEngineFramework.SlimDX, viewModel.AvailableFrameworks[2]);
			Assert.AreEqual(DeltaEngineFramework.Xna, viewModel.AvailableFrameworks[3]);
		}

		[Test]
		public void ChangeSelection()
		{
			viewModel.OnFrameworkSelectionChanged.Execute(1);
			Assert.AreEqual(DeltaEngineFramework.SharpDX, viewModel.SelectedFramework);
			viewModel.OnFrameworkSelectionChanged.Execute(2);
			Assert.AreEqual(DeltaEngineFramework.SlimDX, viewModel.SelectedFramework);
			viewModel.OnFrameworkSelectionChanged.Execute(3);
			Assert.AreEqual(DeltaEngineFramework.Xna, viewModel.SelectedFramework);
			viewModel.OnFrameworkSelectionChanged.Execute(0);
			Assert.AreEqual(DeltaEngineFramework.OpenTK, viewModel.SelectedFramework);
		}

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