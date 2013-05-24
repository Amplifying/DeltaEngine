using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the user input validation.
	/// </summary>
	public class InputValidatorTests
	{
		[TestCase("Name"), TestCase("ProjectName")]
		public void ValidFolderNames(string name)
		{
			Assert.IsTrue(InputValidator.IsValidFolderName(name));
		}

		[TestCase(""), TestCase("name"), TestCase("1Name"), TestCase("Project Name")]
		public void InvalidFolderNames(string name)
		{
			Assert.IsFalse(InputValidator.IsValidFolderName(name));
		}

		[TestCase("C:\\DeltaEngine\\"), TestCase("c:\\deltaengine\\"), TestCase("\\DeltaEngine\\")]
		public void ValidPaths(string path)
		{
			Assert.IsTrue(InputValidator.IsValidPath(path));
		}

		[TestCase("DeltaEngine\\"), TestCase("\\DeltaEngine"), TestCase("\\c:\\DeltaEngine\\"),
		 TestCase("\\Delta Engine\\"), TestCase("C:\\Delta Engine\\")]
		public void InvalidPaths(string path)
		{
			Assert.IsFalse(InputValidator.IsValidPath(path));
		}
	}
}