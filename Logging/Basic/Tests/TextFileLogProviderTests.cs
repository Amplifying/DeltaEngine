using System;
using System.IO;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Logging.Basic.Tests
{
	[Category("Slow")]
	internal class TextFileLogProviderTests
	{
		//ncrunch: no coverage start
		[SetUp]
		public void CreateProvider()
		{
			File.Delete(LogFilePath);
			provider = new TextFileLogProvider();
		}

		private TextFileLogProvider provider;

		[TearDown]
		public void DisposeProvider()
		{
			provider.Dispose();
		}

		[Test]
		public void WhenThereWasNoLoggingNoFileIsCreated()
		{
			Assert.IsFalse(File.Exists(LogFilePath));
		}

		private static string LogFilePath
		{
			get
			{
				return
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
						"Delta Engine", AssemblyExtensions.DetermineProjectName() + ".txt");
			}
		}

		[Test, Category("Slow")]
		public void LogInfoAndOpenFile()
		{
			provider.Log(new Info("Test for logging info"));
		}

		[Test, Category("Slow")]
		public void LogWarningAndOpenFile()
		{
			provider.Log(new Warning("Something strange happened"));
		}

		[Test, Category("Slow")]
		public void LogErrorAndOpenFile()
		{
			provider.Log(new Error(new InsufficientMemoryException()));
		}
	}
}