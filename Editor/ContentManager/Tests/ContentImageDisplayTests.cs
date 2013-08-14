using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests
{
	internal class ContentImageDisplayTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			var fileSystem =
				new MockFileSystem(new Dictionary<string, MockFileData>
				{
					{ @"Content\DeltaEngineLogo.png", new MockFileData(DataToString(@"DeltaEngineLogo.png"))},
				});
			contentImageDisplay = new ImagePreviewer();
			contentImageDisplay.PreviewContent("DeltaEngineLogo");
		}

		private static string DataToString(string path)
		{
			var fileSystem = new FileSystem();
			return fileSystem.File.ReadAllText(path);
		}

		private ImagePreviewer contentImageDisplay;
	
		[Test]
		public void MoveCamera()
		{
			var mockMouse = Resolve<Mouse>() as MockMouse;
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Point(0.2f, 0.2f));
		}

		[Test]
		public void ZoomCamera()
		{
			var mockMouse = Resolve<Mouse>() as MockMouse;
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetPosition(new Point(0.2f, 0.2f));
		}		
	}
}