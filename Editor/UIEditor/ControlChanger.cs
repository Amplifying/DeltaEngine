using DeltaEngine.Datatypes;
using DeltaEngine.Scenes.Controls;
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
			rect.Width = uiControl.EntityWidth;
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
			rect.Height = uiControl.EntityHeight;
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
			if (selectedEntity2D.GetType() == typeof(Label))
				ChangeLabel((Label)selectedEntity2D, uiControl);
		}

		private static void ChangeLabel(Label label, UIControl uiControl)
		{
			label.Size = new Size(uiControl.EntityWidth, uiControl.EntityHeight);
			label.Text = uiControl.contentText;
		}

		public void SetControlLayer(int value, UIControl uiControl, UIEditorScene uiEditorScene)
		{
			var selectedEntity2D = uiEditorScene.SelectedEntity2D;
			uiControl.controlLayer = value < 0 ? 0 : value;
			if (selectedEntity2D == null)
				return;
			selectedEntity2D.RenderLayer = uiControl.controlLayer;
		}

		public void SetSelectedControlNameInList(string value, UIControl uiControl,
			UIEditorScene uiEditorScene)
		{
			if (value == null || uiControl.Index < 0 || uiEditorScene.Scene.Controls.Count <= 0)
				return;
			uiEditorScene.SelectedControlNameInList = value;
			uiEditorScene.SelectedEntity2D = uiEditorScene.Scene.Controls[uiControl.Index];
			uiEditorScene.UpdateOutLine(uiEditorScene.SelectedEntity2D);
			uiControl.ControlName = value;
			uiControl.controlLayer = uiEditorScene.SelectedEntity2D.RenderLayer;
			uiControl.EntityWidth = uiEditorScene.SelectedEntity2D.DrawArea.Width;
			uiControl.EntityHeight = uiEditorScene.SelectedEntity2D.DrawArea.Height;
			if (uiEditorScene.SelectedEntity2D.GetType() == typeof(Button))
				uiControl.contentText = ((Button)uiEditorScene.SelectedEntity2D).Text;
			else if (uiEditorScene.SelectedEntity2D.GetType() == typeof(Label))
				uiControl.contentText = ((Label)uiEditorScene.SelectedEntity2D).Text;
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
			uiEditorScene.SelectedControlNameInList = controlName;
			selectedEntity2D.ClearTags();
			selectedEntity2D.AddTag(controlName);
		}
	}
}