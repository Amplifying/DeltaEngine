using System;
using System.IO;
using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the Delta Engine C# project.
	/// </summary>
	public class CsProjectTests
	{
		[SetUp]
		public void Init()
		{
			project = new CsProject("John Doe");
		}

		private CsProject project;

		[Test]
		public void DefaultStarterKitIsGhostWars()
		{
			Assert.AreEqual("GhostWars", project.StarterKit);
		}

		[Test]
		public void DefaultProjectNameIsUserNamePlusStarterKitName()
		{
			Assert.AreEqual("JohnDoesGhostWars", project.Name);
		}

		[Test]
		public void ChangingStarterKitOnlyChangesTheNameIfItHasNotBeenChangedYet()
		{
			project.StarterKit = "LogoApp";
			Assert.AreEqual("JohnDoesLogoApp", project.Name);
			project.StarterKit = "Snake";
			Assert.AreEqual("JohnDoesSnake", project.Name);
			project.Name = "NewDeltaEngineProject";
			Assert.AreEqual("NewDeltaEngineProject", project.Name);
			project.StarterKit = "Asteroids";
			Assert.AreEqual("NewDeltaEngineProject", project.Name);
		}

		[Test]
		public void DefaultFrameworkIsGLFW()
		{
			Assert.AreEqual(DeltaEngineFramework.GLFW, project.Framework);
		}

		[Test]
		public void DefaultPathIsDefaultVisualStudioProjectLocation()
		{
			Assert.IsTrue(project.Path.Contains("Visual Studio") && project.Path.Contains("Projects"));
			Assert.IsTrue(Directory.Exists(project.Path));
		}

		[Test]
		public void VisualStudioProjectsFolderInMyDocumentsMustBeAvailable()
		{
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string visualStudioProjectsPath = Path.Combine(myDocumentsPath, "Visual Studio 2012",
				"Projects");
			Assert.AreEqual(visualStudioProjectsPath, CsProject.GetVisualStudioProjectsFolder());
			Assert.True(Directory.Exists(CsProject.GetVisualStudioProjectsFolder()));
		}
	}
}