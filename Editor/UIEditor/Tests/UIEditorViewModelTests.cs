using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class UIEditorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Init()
		{
			mockService = new MockService("TestUser", "NewProjectWithoutContent");
			viewModel = new UIEditorViewModel(mockService);
			viewModel.ClearScene("");
		}

		private UIEditorViewModel viewModel;
		private MockService mockService;

		[Test, CloseAfterFirstFrame]
		public void NewImageShouldBeAdded()
		{
			AddNewSprite();
			Assert.AreEqual(1, viewModel.Scene.Controls.Count);
		}

		private void AddNewSprite()
		{
			viewModel.Adder.SetDraggingImage(true);
			viewModel.Adder.AddImage(new Vector2D(0.5f, 0.5f), viewModel.uiControl,
				viewModel.uiEditorScene);
			viewModel.SelectedControlNameInList = "NewSprite0";
		}

		[Test, CloseAfterFirstFrame]
		public void CannotAddIfImageDraggingNotSet()
		{
			viewModel.Adder.SetDraggingImage(false);
			viewModel.Adder.AddImage(new Vector2D(1.0f, 1.0f), viewModel.uiControl,
				viewModel.uiEditorScene);
			Assert.AreEqual(0, viewModel.Scene.Controls.Count);
			Assert.AreEqual(2, viewModel.ContentImageListList.Count);
		}

		[Test]
		public void UIShouldSave()
		{
			AddNewSprite();
			viewModel.UIName = "SceneWithAButton";
			viewModel.SaveUI("");
			Assert.AreEqual(1, ((MockService)viewModel.service).NumberOfMessagesSent);
		}

		[Test]
		public void CannotSaveUIIfNoControlsWereAdded()
		{
			viewModel.UIName = "SceneWithAButton";
			bool saved = false;
			mockService.ContentUpdated +=
				(ContentType type, string name) => //ncrunch: no coverage start
				{ saved = CheckNameAndTypeOfUpdate(type, name); }; //ncrunch: no coverage end
			viewModel.SaveUI("");
			Assert.IsFalse(saved);
		}

		[Test]
		public void WillNotLoadNonExistingScene()
		{
			viewModel.UIName = "NoDataScene";
			bool saved = false;
			mockService.ContentUpdated +=
				(ContentType type, string name) => //ncrunch: no coverage start
				{ saved = CheckNameAndTypeOfUpdate(type, name); }; //ncrunch: no coverage end
			viewModel.SaveUI("");
			Assert.IsFalse(saved);
		}

		//ncrunch: no coverage start
		private static bool CheckNameAndTypeOfUpdate(ContentType type, string name)
		{
			return type == ContentType.Scene && name.Equals("NewUI");
		}//ncrunch: no coverage end

		[Test, CloseAfterFirstFrame]
		public void GridShouldBeDrawn()
		{
			viewModel.GridWidth = 10;
			viewModel.GridHeight = 10;
			viewModel.IsShowingGrid = true;
			Assert.AreEqual(10, viewModel.GridWidth = 10);
		}

		[Test, CloseAfterFirstFrame]
		public void GridHeightPropertyShouldBe10()
		{
			Assert.AreEqual(10, viewModel.GridHeight = 10);
		}

		[Test, CloseAfterFirstFrame]
		public void IsShowingGridPropertyShouldBeTrue()
		{
			Assert.AreEqual(true, viewModel.IsShowingGrid = true);
		}

		[Test, CloseAfterFirstFrame]
		public void SelectedSpriteNameListShouldHaveDefaultName()
		{
			AddNewSprite();
			Assert.AreEqual("NewSprite0", viewModel.SelectedControlNameInList);
		}

		[Test, CloseAfterFirstFrame]
		public void SelectedInterActiveButtonNameListShouldHaveDefaultName()
		{
			var mouse = Resolve<MockMouse>();
			viewModel.Adder.SetDraggingButton(true);
			viewModel.Adder.AddButton(new Vector2D(0.4f, 0.4f), viewModel.uiControl,
				viewModel.uiEditorScene);
			viewModel.SelectedControlNameInList = viewModel.uiEditorScene.UIImagesInList[0];
			mouse.SetPosition(new Vector2D(0.45f, 0.45f));
			mouse.SetButtonState(MouseButton.Left, State.Pressing);
			viewModel.ContentText = "TestContentText";
			Assert.AreEqual("NewButton0", viewModel.SelectedControlNameInList);
		}

		[Test, CloseAfterFirstFrame]
		public void ControlLayerPropertyShouldBeZero()
		{
			Assert.AreEqual(0, viewModel.ControlLayer);
		}

		[Test, CloseAfterFirstFrame]
		public void GetEntity2DWidthAndHeight()
		{
			Assert.AreEqual(0.1f, viewModel.Entity2DHeight);
			Assert.AreEqual(0.2f, viewModel.Entity2DWidth);
		}

		[Test, CloseAfterFirstFrame]
		public void ContextPropertyIsEmptyString()
		{
			AddNewSprite();
			Assert.AreEqual("", viewModel.ContentText);
		}

		[Test, CloseAfterFirstFrame]
		public void SetSelectedEntity2D()
		{
			var mouse = Resolve<MockMouse>();
			AddNewSprite();
			mouse.SetPosition(new Vector2D(0.45f, 0.45f));
			mouse.SetButtonState(MouseButton.Left, State.Pressing);
			Assert.IsNotNull(viewModel.SelectedEntity2D);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotAddNewButton()
		{
			viewModel.ChangeMaterial("newMaterial2D");
			var mouse = Resolve<MockMouse>();
			viewModel.Adder.SetDraggingButton(false);
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			Assert.AreEqual(0, viewModel.Scene.Controls.Count);
			Assert.AreEqual(2, viewModel.SceneNames.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void NewLabelShouldBeAdded()
		{
			var mouse = Resolve<MockMouse>();
			AddLabel();
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			Assert.AreEqual(1, viewModel.uiEditorScene.Scene.Controls.Count);
		}

		private void AddLabel()
		{
			viewModel.Adder.SetDraggingLabel(true);
			viewModel.Adder.AddLabel(new Vector2D(0.5f, 0.5f), viewModel.uiControl,
				viewModel.uiEditorScene);
			viewModel.SelectedControlNameInList = "NewLabel0";
			Assert.AreEqual("NewLabel0", viewModel.SelectedControlNameInList);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotAddNewLabel()
		{
			var mouse = Resolve<MockMouse>();
			viewModel.Adder.SetDraggingLabel(false);
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			Assert.AreEqual(0, viewModel.Scene.Controls.Count);
			Assert.AreEqual(0, viewModel.UIImagesInList.Count);
			Assert.AreEqual(2, viewModel.MaterialList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void DeleteUIElement()
		{
			AddNewSprite();
			AddNewSprite();
			viewModel.uiControl.Index = 1;
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.Delete, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			viewModel.DeleteSelectedControl("");
			Assert.AreEqual(1, viewModel.uiEditorScene.Scene.Controls.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImage()
		{
			AddNewSprite();
			var mouse = Resolve<MockMouse>();
			var startPosition = viewModel.SelectedEntity2D.DrawArea.TopLeft;
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			mouse.SetPosition(new Vector2D(0.55f, 0.55f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(startPosition, viewModel.SelectedEntity2D.DrawArea.TopLeft);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImageWithoutGrid()
		{
			AddNewSprite();
			var startPosition = viewModel.SelectedEntity2D.DrawArea.TopLeft;
			var mouse = Resolve<MockMouse>();
			mouse.SetPosition(new Vector2D(0.55f, 0.55f));
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(startPosition, viewModel.SelectedEntity2D.DrawArea.TopLeft);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImageUsingGrid()
		{
			AddNewSprite();
			AddNewSprite();
			var startPosition = viewModel.uiEditorScene.Scene.Controls[0].TopLeft;
			viewModel.IsSnappingToGrid = true;
			viewModel.GridWidth = 1;
			viewModel.GridHeight = 1;
			var mouse = Resolve<MockMouse>();
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			Assert.AreEqual(startPosition, viewModel.uiEditorScene.Scene.Controls[1].TopLeft);
			Assert.IsTrue(viewModel.IsSnappingToGrid);
		}

		[Test, CloseAfterFirstFrame]
		public void IsShowingGridPropertyShouldBeFalse()
		{
			Assert.IsFalse(viewModel.IsShowingGrid);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMaterial()
		{
			viewModel.ChangeMaterial("newMaterial2D");
			AddNewButton();
			viewModel.ChangeMaterial("newMaterial2D");
			viewModel.SelectedEntity2D = new Sprite(ContentLoader.Load<Material>("material"),
				Rectangle.One);
			viewModel.ChangeMaterial("newMaterial2D");
			Assert.AreEqual("newMaterial2D", viewModel.SelectedEntity2D.Get<Material>().Name);
		}

		private void AddNewButton()
		{
			viewModel.Adder.SetDraggingButton(true);
			viewModel.Adder.AddButton(new Vector2D(0.5f, 0.5f), viewModel.uiControl,
				viewModel.uiEditorScene);
			viewModel.SelectedControlNameInList = "NewButton0";
			Assert.AreEqual("NewButton0", viewModel.SelectedControlNameInList);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRenderLayer()
		{
			viewModel.uiEditorScene.SelectedEntity2D =
				viewModel.controlAdder.AddNewImageToList(Vector2D.Half, viewModel.uiControl,
					viewModel.uiEditorScene);
			viewModel.ChangeRenderLayer(1);
			Assert.AreEqual(1, viewModel.ControlLayer);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRenderLayerWhenNothingIsSelectedWillNotCrash()
		{
			viewModel.SelectedEntity2D = null;
			viewModel.ChangeRenderLayer(1);
			Assert.AreEqual(1, viewModel.ControlLayer);
		}

		[TestCase("10 x 10", 10), TestCase("16 x 16", 16), TestCase("20 x 20", 20),
		 TestCase("24 x 24", 24), TestCase("50 x 50", 50)]
		public void ChangeGrid(string widthAndHeight, int width)
		{
			viewModel.SelectedResolution = widthAndHeight;
			Assert.AreEqual(width.ToString(), viewModel.GridWidth.ToString());
		}

		[Test, CloseAfterFirstFrame]
		public void DeletingAControlAndAddingANewWillTakeItsPlace()
		{
			AddNewSprite();
			AddNewSprite();
			viewModel.SelectedControlNameInList = viewModel.uiEditorScene.UIImagesInList[0];
			viewModel.DeleteSelectedControl("");
			AddNewSprite();
		}

		[Test, CloseAfterFirstFrame]
		public void StartingEditorWillLoadSceneNames()
		{
			Assert.AreEqual(2, viewModel.uiEditorScene.SceneNames.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlName()
		{
			AddNewSprite();
			Assert.AreEqual("NewSprite0", viewModel.ControlName);
			viewModel.ControlName = "TestName";
			Assert.AreEqual("TestName", viewModel.ControlName);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlNameAndAddNewSprite()
		{
			AddNewSprite();
			AddNewSprite();
			Assert.AreEqual("NewSprite0", viewModel.ControlName);
			viewModel.ControlName = "TestName";
			Assert.AreEqual("TestName", viewModel.ControlName);
			viewModel.uiEditorScene.UIImagesInList[0] = null;
			AddNewSprite();
			Assert.AreEqual("NewSprite0", viewModel.uiEditorScene.UIImagesInList[2]);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlNameAndAddNewButton()
		{
			AddNewButton();
			AddNewButton();
			Assert.AreEqual("NewButton0", viewModel.ControlName);
			viewModel.ControlName = "TestName";
			Assert.AreEqual("TestName", viewModel.ControlName);
			viewModel.uiEditorScene.UIImagesInList[0] = null;
			AddNewButton();
			Assert.AreEqual("NewButton0", viewModel.uiEditorScene.UIImagesInList[2]);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlNameAndAddNewLabel()
		{
			AddNewLabel();
			AddNewLabel();
			Assert.AreEqual("NewLabel0", viewModel.ControlName);
			viewModel.ControlName = "TestName";
			Assert.AreEqual("TestName", viewModel.ControlName);
			viewModel.uiEditorScene.UIImagesInList[0] = null;
			AddNewLabel();
			Assert.AreEqual("NewLabel0", viewModel.uiEditorScene.UIImagesInList[2]);
		}

		private void AddNewLabel()
		{
			viewModel.Adder.SetDraggingLabel(true);
			viewModel.Adder.AddLabel(new Vector2D(0.5f, 0.5f), viewModel.uiControl,
				viewModel.uiEditorScene);
			viewModel.SelectedControlNameInList = "NewLabel0";
			Assert.AreEqual("NewLabel0", viewModel.SelectedControlNameInList);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlNameAndAddNewSlider()
		{
			AddNewSlider();
			AddNewSlider();
			Assert.AreEqual(NewSliderId, viewModel.ControlName);
			viewModel.ControlName = "TestName";
			Assert.AreEqual("TestName", viewModel.ControlName);
			viewModel.uiEditorScene.UIImagesInList[0] = null;
			AddNewSlider();
			Assert.AreEqual(NewSliderId, viewModel.uiEditorScene.UIImagesInList[2]);
		}

		private const string NewSliderId = "NewSlider0";

		private void AddNewSlider()
		{
			viewModel.Adder.SetDraggingSlider(true);
			viewModel.Adder.AddSlider(new Vector2D(0.5f, 0.5f), viewModel.uiControl,
				viewModel.uiEditorScene);
			viewModel.SelectedControlNameInList = NewSliderId;
			Assert.AreEqual(NewSliderId, viewModel.SelectedControlNameInList);
		}
	}
}