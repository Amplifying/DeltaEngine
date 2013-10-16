using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Helpers
{
	public class DesignEditorPlugin : EditorPluginView
	{
		public void Init(Service service) {}

		public void ProjectChanged() {}

		public string ShortName
		{
			get { return "Test Plugin"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Content.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}
	}
}