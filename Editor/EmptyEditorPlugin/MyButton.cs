using DeltaEngine.Editor.Common.Properties;

namespace DeltaEngine.Editor.EmptyEditorPlugin
{
	public class MyButton
	{
		public MyButton()
		{
			Text = Resources.ClickMe;
		}

		public string Text { get; set; }
	}
}