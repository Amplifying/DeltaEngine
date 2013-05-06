using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.Tests
{
	public class MockEditorPluginView /*: EditorPluginView*/
	{
		public string Icon
		{
			get { return ""; }
		}
		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Settings; }
		}
	}
}