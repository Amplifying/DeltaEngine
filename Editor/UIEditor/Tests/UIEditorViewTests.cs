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
			uiEditor =
				new UIEditorViewModel(new MockService("TestUser",
					"DeltaEngine.Editor.ImageAnimationEditor.Tests"));
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
		}

		private UIEditorViewModel uiEditor;
		private MockMouse mockMouse;

		[Test]
		public void AddControlsToScene()
		{
			uiEditor.AddImage("NewImage");
			Assert.AreEqual(1, uiEditor.scene.Controls.Count);
			uiEditor.AddImage("NewImage");
			Assert.AreEqual(2, uiEditor.scene.Controls.Count);
		}

		[Test]
		public void SelectControlInUiControlList()
		{
			uiEditor.AddImage("NewImage");
			uiEditor.SelectedSpriteNameInList = "NewImage";
			Assert.AreEqual(4, uiEditor.ControlProcessor.outLines.Length);
		}

		[Test]
		public void MoveControlWithoutGrid()
		{
			uiEditor.AddImage("NewImage");
			uiEditor.SelectedSpriteNameInList = "NewImage";
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void MoveControlWithGrid()
		{
			uiEditor.GridHeight = 10;
			uiEditor.GridWidth = 10;
			uiEditor.AddImage("NewImage");
			uiEditor.SelectedSpriteNameInList = "NewImage";
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetPosition(new Point(1f, 1f));
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void SaveUI()
		{
			uiEditor.GridHeight = 10;
			uiEditor.GridWidth = 10;
			uiEditor.AddImage("NewImage");
			uiEditor.SelectedSpriteNameInList = "NewImage";
			uiEditor.UIName = "NewUI";
			uiEditor.SaveUI("");
		}

		[Test]
		public void SelectControlByClicking()
		{
			uiEditor.AddImage("NewImage");
			mockMouse.SetButtonState(MouseButton.Left, State.Pressing);
			mockMouse.SetPosition(new Point(0.5f, 0.5f));
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void RemovingSelectedControl()
		{
			uiEditor.AddImage("NewImage");
			uiEditor.AddImage("NewImage");
			uiEditor.SelectedSpriteNameInList = "NewImage";
			Assert.AreEqual(2, uiEditor.scene.Controls.Count);
			var mockKeyboard = Resolve<Keyboard>() as MockKeyboard;
			mockKeyboard.SetKeyboardState(Key.Delete, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(1, uiEditor.scene.Controls.Count);
		}

		[Test]
		public void ChangeMaterialOfControl()
		{
			
		}
	}
}