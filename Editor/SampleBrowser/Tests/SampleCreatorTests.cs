using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	public class SampleCreatorTests
	{
		[SetUp]
		public void Init()
		{
			sampleCreator = new SampleCreator(CreateMockSamplesFileSystem());
		}

		private SampleCreator sampleCreator;

		private IFileSystem CreateMockSamplesFileSystem()
		{
			var files = new Dictionary<string, MockFileData>();
			files.Add(basePath + @"\Samples\EmptyApp\EmptyApp.csproj", new MockFileData(""));
			files.Add(basePath + @"\Samples\EmptyApp\bin\Debug\EmptyApp.exe", new MockFileData(""));
			files.Add(basePath + @"\Samples\EmptyApp\Tests\EmptyApp.Tests.csproj", new MockFileData(""));
			files.Add(basePath + @"\Editor\SampleBrowser\Tests\Assemblies\EmptyApp.Tests.dll",
				new MockFileData(GetTestAssemblyData()));
			files.Add(basePath + @"\Tutorials\Basic01CreateWindow\Program.cs", new MockFileData(""));
			return new MockFileSystem(files);
		}

		private readonly string basePath = PathExtensions.GetFallbackEngineSourceCodeDirectory();

		private static string GetTestAssemblyData()
		{
			string assembly = Path.GetFullPath("TestAssembly.dll");
			return File.ReadAllText(assembly);
		}

		[Test, Category("Slow")]
		public void CreateSampleFromMockAssembly()
		{
			Assert.AreEqual(0, sampleCreator.Samples.Count);
			sampleCreator.CreateSamples(DeltaEngineFramework.Default);
			Assert.AreEqual(1, sampleCreator.Samples.Count);
			Assert.AreEqual("EmptyApp", sampleCreator.Samples[0].Title);
			Assert.AreEqual("Sample Game", sampleCreator.Samples[0].Description);
			Assert.AreEqual("Game", sampleCreator.Samples[0].Category.ToString());
			Assert.AreEqual("http://DeltaEngine.net/Editor/Icons/EmptyApp.png",
				sampleCreator.Samples[0].ImageUrl);
			Assert.AreEqual(basePath + @"\Samples\EmptyApp\EmptyApp.csproj",
				sampleCreator.Samples[0].ProjectFilePath);
			Assert.AreEqual(basePath + @"\Samples\EmptyApp\bin\Debug\EmptyApp.exe",
				sampleCreator.Samples[0].AssemblyFilePath);
			Assert.AreEqual(null, sampleCreator.Samples[0].EntryClass);
			Assert.AreEqual(null, sampleCreator.Samples[0].EntryMethod);
		}
	}
}