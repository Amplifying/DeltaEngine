using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	[Category("Slow")]
	public class UIEditorViewTests
	{
		[SetUp]
		public void SetUp()
		{
			contentPath = "Content";
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{
						@"Content\DeltaEngineLogo.png",
						new MockFileData(DataToString(Path.Combine(contentPath, @"DeltaEngineLogo.png")))
					},
				});
			var uiEditor = new UIEditorViewModel();
		}

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		private string contentPath;

		[Test]
		public void DrawImage()
		{
			//uiEditor.AddImage();
		}
	}
}