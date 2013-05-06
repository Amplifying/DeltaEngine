using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the Visual Studio Template.
	/// </summary>
	public class VsTemplateTests
	{
		[Test]
		public void CreateWithEmptyGameTemplate()
		{
			var template = VsTemplate.GetEmptyGame();
			Assert.AreEqual(TemplateZip, template.PathToZip);
			Assert.AreEqual(AssemblyInfo, template.AssemblyInfo);
			Assert.AreEqual(Csproj, template.Csproj);
			Assert.AreEqual(Ico, template.Ico);
			Assert.AreEqual(2, template.SourceCodeFiles.Count);
			Assert.AreEqual(Program, template.SourceCodeFiles[0]);
			Assert.AreEqual(Game, template.SourceCodeFiles[1]);
		}

		private const string TemplateZip =
			"C:\\Code\\DeltaEngine\\VisualStudioTemplates\\Delta Engine\\EmptyGame.zip";
		private const string AssemblyInfo = "Properties/AssemblyInfo.cs";
		private const string Csproj = "EmptyGame.csproj";
		private const string Ico = "EmptyGameIcon.ico";
		private const string Program = "Program.cs";
		private const string Game = "Game.cs";

		[Test]
		public void CheckTotalNumberOfFilesFromEmptyGameTemplate()
		{
			var template = VsTemplate.GetEmptyGame();
			var list = template.GetAllFilePathsAsList();
			Assert.AreEqual(5, list.Count);
		}
	}
}