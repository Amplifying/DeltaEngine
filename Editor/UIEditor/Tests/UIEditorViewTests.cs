using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	[Category("Slow")]
	public class UIEditorViewTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			uiEditorViewModel =
				new UIEditorViewModel(new MockService("TestUser", "NewProjectWithContent"));
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
		}

		private UIEditorViewModel uiEditorViewModel;
		private MockMouse mockMouse;

		[Test]
		public void AddControlsToScene()
		{
			uiEditorViewModel.AddImage(Vector2D.Half);
			Assert.AreEqual(1, uiEditorViewModel.scene.Controls.Count);
			uiEditorViewModel.AddImage(Vector2D.Half);
			Assert.AreEqual(2, uiEditorViewModel.scene.Controls.Count);
		}

		[Test]
		public void SelectControlInUiControlList()
		{
			uiEditorViewModel.AddImage(Vector2D.Half);
			uiEditorViewModel.SelectedSpriteNameInList = "NewImage";
			Assert.AreEqual(4, uiEditorViewModel.ControlProcessor.outLines.Length);
		}

		[Test]
		public void MoveControlWithoutGrid()
		{
			uiEditorViewModel.AddImage(Vector2D.Half);
			uiEditorViewModel.SelectedSpriteNameInList = "NewImage";
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void MoveControlWithGrid()
		{
			uiEditorViewModel.GridHeight = 10;
			uiEditorViewModel.GridWidth = 10;
			uiEditorViewModel.AddImage(Vector2D.Half);
			uiEditorViewModel.SelectedSpriteNameInList = "NewImage";
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void SaveUI()
		{
			uiEditorViewModel.GridHeight = 10;
			uiEditorViewModel.GridWidth = 10;
			uiEditorViewModel.AddImage(Vector2D.Half);
			uiEditorViewModel.SelectedSpriteNameInList = "NewImage";
			uiEditorViewModel.UIName = "NewUI";
			uiEditorViewModel.SaveUI("");
		}

		[Test]
		public void SelectControlByClicking()
		{
			uiEditorViewModel.AddImage(Vector2D.Half);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressing);
			mockMouse.SetPosition(new Vector2D(0.5f, 0.5f));
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void RemovingSelectedControl()
		{
			uiEditorViewModel.AddImage(Vector2D.Half);
			uiEditorViewModel.AddImage(Vector2D.Half);
			uiEditorViewModel.SelectedSpriteNameInList = "NewImage";
			Assert.AreEqual(2, uiEditorViewModel.scene.Controls.Count);
			var mockKeyboard = Resolve<Keyboard>() as MockKeyboard;
			mockKeyboard.SetKeyboardState(Key.Delete, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(1, uiEditorViewModel.scene.Controls.Count);
		}

		[Test]
		public void AllMaterialsShouldBe2D()
		{
			Assert.AreEqual(1, uiEditorViewModel.MaterialList.Count);
			Assert.AreEqual("MyMaterial1In2D", uiEditorViewModel.MaterialList[0]);
		}
	}
}