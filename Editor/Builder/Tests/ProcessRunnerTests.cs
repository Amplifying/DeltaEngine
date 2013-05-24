using System;
using NUnit.Framework;

namespace DeltaEngine.Editor.Builder.Tests
{
	public class ProcessRunnerTests
	{
		[Test]
		public void DefaultWorkingDirectory()
		{
			var processRunner = new ProcessRunner("dir");
			Assert.AreEqual(Environment.CurrentDirectory, processRunner.WorkingDirectory);
		}

		[Test]
		public void ChangingWorkingDirectory()
		{
			var processRunner = new ProcessRunner("dir");
			processRunner.Start();
			var outputWithDefaultWorkingDirectory = processRunner.Output;
			processRunner.WorkingDirectory = @"C:\";
			processRunner.Start();
			var outputWithDefinedWorkingDirectory = processRunner.Output;
			Assert.AreNotEqual(outputWithDefaultWorkingDirectory, outputWithDefinedWorkingDirectory);
		}
	}
}