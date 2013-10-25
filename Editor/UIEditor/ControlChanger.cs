using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Scenes.UserInterfaces.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlChanger
	{
		public void SetHeight(float value, UIControl uiControl, UIEditorScene uiEditorScene)
		{
			var selectedEntity2D = uiEditorScene.SelectedEntity2D;
			if (uiControl.isClicking || selectedEntity2D == null)
				return;
			uiControl.EntityHeight = value;
			var rect = selectedEntity2D.DrawArea;
			rect.Height = uiControl.EntityHeight;
			selectedEntity2D.DrawArea = rect;
			if (selectedEntity2D.GetType() == typeof(Button))
				ChangeButton((Button)selectedEntity2D, uiControl);
			uiEditorScene.UpdateOutLine(selectedEntity2D);
		}

		public void SetWidth(float value, UIControl uiControl, UIEditorScene uiEditorScene)
		{
			var selectedEntity2D = uiEditorScene.SelectedEntity2D;
			if (uiControl.isClicking || selectedEntity2D == null)
				return;
			uiControl.EntityWidth = value;
			var rect = selectedEntity2D.DrawArea;
			rect.Width = uiControl.EntityWidth;
			selectedEntity2D.DrawArea = rect;
			if (selectedEntity2D.GetType() == typeof(Button))
				ChangeButton((Button)selectedEntity2D, uiControl);
			uiEditorScene.UpdateOutLine(selectedEntity2D);
		}

		private static void ChangeButton(Button button, UIControl uiControl)
		{
			button.Size = new Size(uiControl.EntityWidth, uiControl.EntityHeight);
			button.Text = uiControl.contentText;
		}

		public void SetContentText(string value, UIControl uiControl, UIEditorScene uiEditorScene)
		{
			var selectedEntity2D = uiEditorScene.SelectedEntity2D;
			uiControl.contentText = value;
			if (selectedEntity2D.GetType() == typeof(Button))
				ChangeButton((Button)selectedEntity2D, uiControl);
		}

		public void SetControlLayer(int value, UIControl uiControl, UIEditorScene uiEditorScene)
		{
			var selectedEntity2D = uiEditorScene.SelectedEntity2D;
			uiControl.controlLayer = value;
			if (selectedEntity2D == null)
				return;
			selectedEntity2D.RenderLayer = uiControl.controlLayer;
		}

		public void SetSelectedSpriteNameInList(string value, UIControl uiControl,
			UIEditorScene uiEditorScene)
		{
			if (string.IsNullOrEmpty(value) || uiControl.Index < 0)
				return;
			uiEditorScene.SelectedSpriteNameInList = value;
			var selectedEntity2D = uiEditorScene.Scene.Controls[uiControl.Index];
			uiEditorScene.UpdateOutLine(selectedEntity2D);
			uiControl.ControlName = value;
			uiControl.controlLayer = selectedEntity2D.RenderLayer;
			uiControl.EntityWidth = selectedEntity2D.DrawArea.Width;
			uiControl.EntityHeight = selectedEntity2D.DrawArea.Height;
			if (selectedEntity2D.GetType() == typeof(Button))
			{
				var button = (Button)selectedEntity2D;
				uiControl.contentText = button.Text;
			}
			else
				uiControl.contentText = "";
		}

		public void ChangeControlName(string controlName, UIControl uiControl,
			UIEditorScene uiEditorScene)
		{
			var selectedEntity2D = uiEditorScene.SelectedEntity2D;
			uiControl.ControlName = controlName;
			Messenger.Default.Send(uiControl.ControlName, "ChangeSelectedControlName");
			var spriteListIndex = uiEditorScene.Scene.Controls.IndexOf(selectedEntity2D);
			if (spriteListIndex < 0)
				return; //ncrunch: no coverage 
			uiEditorScene.UIImagesInList[spriteListIndex] = controlName;
			uiEditorScene.SelectedSpriteNameInList = controlName;
			selectedEntity2D.Get<Material>().MetaData.Name = controlName;
		}
	}
}