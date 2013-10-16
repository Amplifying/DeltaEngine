using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ImageAnimationEditor.Tests
{
	public class AnimationEditorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			editor = new AnimationEditorViewModel(new MockService("TestUser", "DeltaEngine.Editor.ImageAnimationEditor.Tests"));
		}

		private AnimationEditorViewModel editor;

		[Test]
		public void MoveImageUpInTheList()
		{
			editor.SelectedImageToAdd = "Test1";
			editor.SelectedImageToAdd = "Test2";
			editor.MoveImageUp(1);
			Assert.AreEqual("Test2", editor.ImageList[0]);
			editor.MoveImageDown(0);
			Assert.AreEqual("Test1", editor.ImageList[0]);
			editor.MoveImageUp(0);
			editor.MoveImageDown(1);
			editor.RefreshData();
			Assert.AreEqual(1, editor.SelectedIndex);
		}

		[Test]
		public void AddingAndRemovingImagesToList()
		{
			editor.SelectedImageToAdd = "Test1";
			Assert.AreEqual(1,editor.ImageList.Count);
			editor.DeleteImage("Test1");
			Assert.AreEqual(0, editor.ImageList.Count);
		}

		[Test]
		public void SaveAnimation()
		{
			editor.SelectedImageToAdd = "Test1";
			editor.SelectedImageToAdd = "Test2";
			editor.AnimationName = "TestAnimation";
			editor.SaveAnimation("");
		}

		[Test]
		public void SaveSpriteSheet()
		{
			editor.SelectedImageToAdd = "Test1";
			editor.SaveAnimation("");
			editor.AnimationName = "TestAnimation";
			editor.SaveAnimation("");
		}

		[Test]
		public void LoadAnimation()
		{
			editor.AnimationName = "ImageAnimation";
			Assert.AreEqual(3, editor.ImageList.Count);
		}

		[Test]
		public void LoadSpriteSheet()
		{
			editor.AnimationName = "SpriteSheet";
			Assert.AreEqual(1, editor.ImageList.Count);
		}
	}
}