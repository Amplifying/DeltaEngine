using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	class ProjectEntryTests
	{
		[Test]
		public void ParseCSharpProjectDataOfSolutionFile()
		{
			const string ProjectName = "MyCSharpProject";
			const string ProjectFilePath = @"Basics\Coding.MyCSharpProject.csproj";
			const string ProjectGuid = "{AAE7730E-5F62-48D6-B772-C4B1E8665FE1}";
			const string ProjectEntryString =
				"Project(" + ProjectEntry.CSharpProjectTypeGuid + ") = \"" + ProjectName + "\", \"" +
				ProjectFilePath + "\", " + ProjectGuid;

			var projectEntry = new ProjectEntry(ProjectEntryString);
			Assert.IsTrue(projectEntry.IsCSharpProject);
			Assert.IsFalse(projectEntry.IsSolutionFolder);
			Assert.AreEqual(ProjectName, projectEntry.Title);
			Assert.AreEqual(ProjectFilePath, projectEntry.FilePath);
			Assert.AreEqual(ProjectGuid, projectEntry.Guid);
		}

		[Test]
		public void ParseProjectFolderDataOfSolutionFile()
		{
			const string FolderName = "MyFolder";
			const string FolderGuid = "{ACE7730E-5F62-48D6-B772-C4B1E8665FA1}";
			const string ProjectFolderString =
				"Project(" + ProjectEntry.ProjectFolderGuid + ") = \"" + FolderName + "\", \"" +
				FolderName + "\", " + FolderGuid;

			var projectEntry = new ProjectEntry(ProjectFolderString);
			Assert.IsTrue(projectEntry.IsSolutionFolder);
			Assert.IsFalse(projectEntry.IsCSharpProject);
			Assert.AreEqual(FolderName, projectEntry.Title);
			Assert.AreEqual(FolderName, projectEntry.FilePath);
			Assert.AreEqual(FolderGuid, projectEntry.Guid);
		}
	}
}
