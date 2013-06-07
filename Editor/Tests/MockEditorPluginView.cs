using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Tests
{
	public class MockEditorPluginView : EditorPluginView
	{
		public MockEditorPluginView(Service service) {}

		public string ShortName
		{
			get { return "MockEditorPlugin"; }
		}
		public string Icon
		{
			get { return "Mock.png"; }
		}
		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Settings; }
		}
		public int Priority
		{
			get { return 3; }
		}
	}
}