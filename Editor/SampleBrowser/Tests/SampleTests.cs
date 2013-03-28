using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	public class SampleTests
	{
		[Test]
		public void ContainsFilterText()
		{
			Assert.True(testSample.ContainsFilterText("Sample"));
			Assert.False(testSample.ContainsFilterText("Test"));
		}

		private readonly Sample testSample = new Sample
		{
			Title = "Sample's Name",
			Description = "Simulate how a real sample would looks like in the Sample Browser.",
			DifficultyLevel = "Easy",
			ImageFilePath = "http://DeltaEngine.net/Content/Icons/Rendering.png",
			ProjectFilePath = "Delta.Rendering.Tutorials",
			ExecutableFilePath = "",
			AssemblyNamespace = "",
			EntryPointMethod = "",
			AllowLauncher = true
		};

		[Test]
		public new void ToString()
		{
			Assert.AreEqual(
				"Sample: Title=Sample's Name, " + "Difficulty=Easy, " +
					"Description=Simulate how a real sample would looks like in the Sample Browser.",
				testSample.ToString());
		}
	}
}