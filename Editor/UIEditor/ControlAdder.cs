using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.UserInterfaces.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlAdder
	{
		public void SetDraggingImage(bool draggingImage)
		{
			IsDraggingImage = draggingImage;
			IsDragging = draggingImage;
		}

		public bool IsDraggingImage { get; set; }
		public bool IsDragging { get; set; }

		public void SetDraggingButton(bool draggingButton)
		{
			IsDraggingButton = draggingButton;
			IsDragging = draggingButton;
		}

		public bool IsDraggingButton { get; set; }

		public void SetDraggingLabel(bool draggingLabel)
		{
			IsDraggingLabel = draggingLabel;
			IsDragging = draggingLabel;
		}

		public bool IsDraggingLabel { get; set; }

		public void SetDraggingSlider(bool draggingSlider)
		{
			IsDraggingSlider = draggingSlider;
			IsDragging = draggingSlider;
		}

		public bool IsDraggingSlider { get; set; }

		public void AddImage(Vector2D position, UIControl uiControl, UIEditorScene uiEditorScene)
		{
			if (!IsDraggingImage)
				return;
			var sprite = AddNewImageToList(position, uiControl, uiEditorScene);
			uiEditorScene.SelectedEntity2D = sprite;
			uiControl.EntityWidth = sprite.DrawArea.Width;
			uiControl.EntityHeight = sprite.DrawArea.Height;
			IsDraggingImage = false;
			IsDragging = false;
			Messenger.Default.Send(sprite.Material.MetaData.Name, "AddToHierachyList");
		}

		public Sprite AddNewImageToList(Vector2D position, UIControl uiControl,
			UIEditorScene uiEditorScene)
		{
			var newSprite = CreateANewDefaultImage(position);
			uiEditorScene.Scene.Add(newSprite);
			SetDefaultNamesOfNewImages(newSprite, uiEditorScene);
			return newSprite;
		}

		private static Sprite CreateANewDefaultImage(Vector2D position)
		{
			var costumImage = CreateDefaultImage();
			var material = new Material(ContentLoader.Load<Shader>(Shader.Position2DColorUV),
				costumImage, costumImage.PixelSize);
			var newSprite = new Sprite(material,
				Rectangle.FromCenter(position, new Size(0.05f)));
			return newSprite;
		}

		private static void SetDefaultNamesOfNewImages(Sprite newSprite, UIEditorScene uiEditorScene)
		{
			bool freeName = false;
			int numberOfNames = 0;
			while (freeName == false)
				if (uiEditorScene.UIImagesInList.Contains("NewSprite" + numberOfNames))
					numberOfNames++;
				else
					freeName = true;
			uiEditorScene.UIImagesInList.Add("NewSprite" + numberOfNames);
			if (uiEditorScene.UIImagesInList[0] == null)
				uiEditorScene.UIImagesInList[0] = "NewSprite" + numberOfNames;
			newSprite.Material.MetaData.Name = "NewSprite" + numberOfNames;
		}

		private static Image CreateDefaultImage()
		{
			var creationData = new ImageCreationData(new Size(8));
			var colors = new Color[8 * 8];
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if ((i + j) % 2 == 0)
						colors[i * 8 + j] = Color.LightGray;
					else
						colors[i * 8 + j] = Color.DarkGray;
			var costumImage = ContentLoader.Create<Image>(creationData);
			costumImage.Fill(colors);
			return costumImage;
		}

		public void AddButton(Vector2D position, UIControl uiControl, UIEditorScene uiEditorScene)
		{
			if (!IsDraggingButton)
				return;
			var button = AddNewButtonToList(position, uiEditorScene);
			uiEditorScene.SelectedEntity2D = button;
			uiControl.contentText = "Default Button";
			uiControl.EntityWidth = button.DrawArea.Width;
			uiControl.EntityHeight = button.DrawArea.Height;
			IsDraggingButton = false;
			IsDragging = false;
			Messenger.Default.Send(button.Get<Theme>().Button.MetaData.Name, "AddToHierachyList");
		}

		private static Entity2D AddNewButtonToList(Vector2D position, UIEditorScene uiEditorScene)
		{
			var newButton = new Button(Rectangle.FromCenter(position, new Size(0.2f, 0.1f)),
				"Default Button");
			uiEditorScene.Scene.Add(newButton);
			SetDefaultButtonName(newButton, uiEditorScene);
			return newButton;
		}

		private static void SetDefaultButtonName(Button newButton,
			UIEditorScene uiEditorScene)
		{
			bool freeName = false;
			int numberOfNames = 0;
			while (freeName == false)
				if (uiEditorScene.UIImagesInList.Contains("NewButton" + numberOfNames))
					numberOfNames++;
				else
					freeName = true;
			uiEditorScene.UIImagesInList.Add("NewButton" + numberOfNames);
			if (uiEditorScene.UIImagesInList[0] == null)
				uiEditorScene.UIImagesInList[0] = "NewButton" + numberOfNames;
			SetNameOfAllButtonStates(newButton, numberOfNames);
		}

		private static void SetNameOfAllButtonStates(Button newButton, int numberOfNames)
		{
			newButton.Get<Theme>().Button.MetaData.Name = "NewButton" + numberOfNames;
			newButton.Get<Theme>().ButtonDisabled.MetaData.Name = "NewButton" + numberOfNames;
			newButton.Get<Theme>().ButtonMouseover.MetaData.Name = "NewButton" + numberOfNames;
			newButton.Get<Theme>().ButtonPressed.MetaData.Name = "NewButton" + numberOfNames;
		}

		public void AddLabel(Vector2D position, UIControl uiControl, UIEditorScene uiEditorScene)
		{
			if (!IsDraggingLabel)
				return;
			var label = AddNewLabelToList(position, uiEditorScene);
			uiEditorScene.SelectedEntity2D = label;
			uiControl.contentText = "Default Label";
			uiControl.EntityWidth = label.DrawArea.Width;
			uiControl.EntityHeight = label.DrawArea.Height;
			IsDraggingLabel = false;
			IsDragging = false;
			Messenger.Default.Send(label.Material.MetaData.Name, "AddToHierachyList");
		}

		private static Label AddNewLabelToList(Vector2D position, UIEditorScene uiEditorScene)
		{
			var newLabel = new Label(Rectangle.FromCenter(position, new Size(0.2f, 0.1f)), "DefaultLabel");
			uiEditorScene.Scene.Add(newLabel);
			SetDefaultNameOfLable(newLabel, uiEditorScene);
			return newLabel;
		}

		private static void SetDefaultNameOfLable(Label newLabel, UIEditorScene uiEditorScene)
		{
			bool freeName = false;
			int numberOfNames = 0;
			while (freeName == false)
				if (uiEditorScene.UIImagesInList.Contains("NewLabel" + numberOfNames))
					numberOfNames++;
				else
					freeName = true;
			uiEditorScene.UIImagesInList.Add("NewLabel" + numberOfNames);
			if (uiEditorScene.UIImagesInList[0] == null)
				uiEditorScene.UIImagesInList[0] = "NewLabel" + numberOfNames;
			newLabel.Material.MetaData.Name = "NewLabel" + numberOfNames;
		}

		public void AddSlider(Vector2D position, UIControl uiControl, UIEditorScene uiEditorScene)
		{
			if (!IsDraggingSlider)
				return;
			var slider = AddNewSliderToList(position, uiEditorScene);
			uiEditorScene.SelectedEntity2D = slider;
			uiControl.contentText = "Default Slider";
			uiControl.EntityWidth = slider.DrawArea.Width;
			uiControl.EntityHeight = slider.DrawArea.Height;
			IsDraggingSlider = false;
			IsDragging = false;
			Messenger.Default.Send(slider.Get<Theme>().Slider.MetaData.Name, "AddToHierachyList");
		}

		private static Slider AddNewSliderToList(Vector2D position, UIEditorScene uiEditorScene)
		{
			var newSlider = new Slider(Rectangle.FromCenter(position, new Size(0.15f, 0.03f)));
			uiEditorScene.Scene.Add(newSlider);
			SetDefaultSliderName(newSlider, uiEditorScene);
			return newSlider;
		}

		private static void SetDefaultSliderName(Slider newSlider, UIEditorScene uiEditorScene)
		{
			bool freeName = false;
			int numberOfNames = 0;
			while (!freeName)
				if (uiEditorScene.UIImagesInList.Contains(NewSliderId + numberOfNames))
					numberOfNames++;
				else
					freeName = true;
			uiEditorScene.UIImagesInList.Add(NewSliderId + numberOfNames);
			if (uiEditorScene.UIImagesInList[0] == null)
				uiEditorScene.UIImagesInList[0] = NewSliderId + numberOfNames;
			SetNameOfAllSliderStates(newSlider, numberOfNames);
		}

		private const string NewSliderId = "NewSlider";

		private static void SetNameOfAllSliderStates(Slider newSlider, int numberOfNames)
		{
			var theme = newSlider.Get<Theme>();
			string name = NewSliderId + numberOfNames;
			theme.Slider.MetaData.Name = name;
			theme.SliderDisabled.MetaData.Name = name;
			theme.SliderPointer.MetaData.Name = name;
			theme.SliderPointerDisabled.MetaData.Name = name;
			theme.SliderPointerMouseover.MetaData.Name = name;
		}
	}
}