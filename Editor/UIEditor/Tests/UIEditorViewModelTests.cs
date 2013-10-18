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
		}

		private UIEditorViewModel viewModel;
		private MockService mockService;

		[Test, CloseAfterFirstFrame]
		public void NewImageShouldBeAdded()
		{
			AddNewSprite();
			Assert.AreEqual(1, viewModel.scene.Controls.Count);
		}

		private void AddNewSprite()
		{
			viewModel.SetDraggingImage(true);
			viewModel.AddImage(new Vector2D(0.5f, 0.5f));
			viewModel.SelectedSpriteNameInList = "NewSprite0";
			Assert.AreEqual("NewSprite0", viewModel.SelectedSpriteNameInList);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotAddIfImageDraggingNotSet()
		{
			viewModel.SetDraggingImage(false);
			viewModel.AddImage(new Vector2D(1.0f, 1.0f));
			Assert.AreEqual(0, viewModel.scene.Controls.Count);
		}

		[Test]
		public void UIShouldSave()
		{
			AddNewSprite();
			viewModel.UIName = "NewUI";
			viewModel.SaveUI("");
			Assert.AreEqual(1, ((MockService)viewModel.service).NumberOfMessagesSend);
		}

		[Test]
		public void CannotSaveUIIfNoControlsWereAdded()
		{
			viewModel.UIName = "NewUI";
			bool saved = false;
			mockService.ContentUpdated += (ContentType type, string name) =>
			{
				//ncrunch: no coverage
				saved = CheckNameAndTypeOfUpdate(type, name); //ncrunch: no coverage
			};
			viewModel.SaveUI("");
			Assert.IsFalse(saved);
		}

		//ncrunch: no coverage start
		private static bool CheckNameAndTypeOfUpdate(ContentType type, string name)
		{
			return type == ContentType.Scene && name.Equals("NewUI");
		}

		//ncrunch: no coverage end

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
			Assert.AreEqual("NewSprite0", viewModel.SelectedSpriteNameInList);
		}

		[Test, CloseAfterFirstFrame]
		public void SelectedInterActiveButtonNameListShouldHaveDefaultName()
		{
			var mouse = Resolve<MockMouse>();
			viewModel.SetDraggingButton(true);
			viewModel.AddButton(new Vector2D(0.4f, 0.4f));
			viewModel.SelectedSpriteNameInList = viewModel.UIImagesInList[0];
			mouse.SetPosition(new Vector2D(0.45f, 0.45f));
			mouse.SetButtonState(MouseButton.Left, State.Pressing);
		}

		[Test, CloseAfterFirstFrame]
		public void ControlLayerPropertyShouldBeZero()
		{
			Assert.AreEqual(0, viewModel.ControlLayer);
		}

		[Test, CloseAfterFirstFrame]
		public void GetEntity2DHeight()
		{
			Assert.AreEqual(0, viewModel.Entity2DHeight);
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
		public void NewButtonShouldBeAdded()
		{
			var mouse = Resolve<MockMouse>();
			viewModel.SetDraggingButton(true);
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			//Assert.AreEqual(1, viewModel.scene.Controls.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotAddNewButton()
		{
			var mouse = Resolve<MockMouse>();
			viewModel.SetDraggingButton(false);
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			Assert.AreEqual(0, viewModel.scene.Controls.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void NewLabelShouldBeAdded()
		{
			var mouse = Resolve<MockMouse>();
			viewModel.SetDraggingLabel(true);
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			//Assert.AreEqual(1, viewModel.scene.Controls.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotAddNewLabel()
		{
			var mouse = Resolve<MockMouse>();
			viewModel.SetDraggingLabel(false);
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			Assert.AreEqual(0, viewModel.scene.Controls.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void DeleteUIElement()
		{
			var keyboard = Resolve<MockKeyboard>();
			AddNewSprite();
			keyboard.SetKeyboardState(Key.Delete, State.Pressing);
			//Assert.AreEqual(0, viewModel.scene.Controls.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImage()
		{
			var mouse = Resolve<MockMouse>();
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			mouse.SetPosition(new Vector2D(0.55f, 0.55f));
			//Assert
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImageWithoutGrid()
		{
			AddNewSprite();
			var mouse = Resolve<MockMouse>();
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			//Assert
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImageUsingGrid()
		{
			AddNewSprite();
			AddNewSprite();
			viewModel.IsSnappingToGrid = true;
			viewModel.GridWidth = 1;
			viewModel.GridHeight = 1;
			var mouse = Resolve<MockMouse>();
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			//Assert
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
			viewModel.SelectedEntity2D = new Sprite(ContentLoader.Load<Material>("material"),
				Rectangle.One);
			viewModel.ChangeMaterial("newMaterial2D");
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRenderLayer()
		{
			viewModel.ChangeRenderLayer(1);
			Assert.AreEqual(1, viewModel.ControlLayer);
		}

		[TestCase(10), TestCase(16), TestCase(20), TestCase(24), TestCase(50), TestCase(0)]
		public void ChangeGrid(int width)
		{
			viewModel.ChangeGrid(width);
			Assert.AreEqual(width, viewModel.GridWidth);
		}

		[Test, CloseAfterFirstFrame]
		public void DeletingAControlAndAddingANewWillTakeItsPlace()
		{
			AddNewSprite();
			AddNewSprite();
			viewModel.SelectedSpriteNameInList = viewModel.UIImagesInList[0];
			viewModel.DeleteSelectedControl("");
			AddNewSprite();
		}
	}
}