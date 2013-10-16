using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class UIToolExtensionsTests
	{
		[Test]
		public void CheckNumberOfUIEditorTools()
		{
			Assert.AreEqual(13, UIToolExtensions.GetNames().Count);
		}

		[TestCase(UITool.Button, "CreateButton.png")]
		[TestCase(UITool.ProgressBar, "CreateProgressBar.png")]
		[TestCase(UITool.Tilemap, "CreateTilemap.png")]
		public void ToolsShouldHaveACorrectImagePath(UITool uiTool, string expectedFilename)
		{
			Assert.AreEqual(Path.Combine("..", "Images", "UIEditor", expectedFilename), 
				uiTool.GetImagePath());
		}
	}
}