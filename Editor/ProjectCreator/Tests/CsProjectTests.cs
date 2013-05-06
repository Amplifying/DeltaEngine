using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the Delta Engine C# project.
	/// </summary>
	public class CsProjectTests
	{
		[Test]
		public void DefaultName()
		{
			var project = new CsProject();
			Assert.AreEqual("NewDeltaEngineProject", project.Name);
		}

		[Test]
		public void DefaultFramework()
		{
			var project = new CsProject();
			Assert.AreEqual(DeltaEngineFramework.OpenTK, project.Framework);
		}

		[Test]
		public void DefaultPath()
		{
			var project = new CsProject();
			Assert.AreEqual(CsProject.GetEnvironmentVariableWithFallback(), project.Location);
		}
	}
}