using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Helpers
{
	public class DesignEditorPlugin : EditorPluginView
	{
		public string ShortName
		{
			get { return "Test Plugin"; }
		}
		public string Icon
		{
			get { return "Icons/Content.png"; }
		}
		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Content; }
		}
		public int Priority
		{
			get { return 1; }
		}
	}
}