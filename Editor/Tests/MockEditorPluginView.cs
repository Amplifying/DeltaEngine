using System;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Tests
{
	public class MockEditorPluginView : EditorPluginView
	{
		public void Init(Service service)
		{
			Console.WriteLine("MockEditorPlugin initialized");
		}

		public void Activate()
		{
			Console.WriteLine("MockEditorPlugin activated");
		}

		public string ShortName
		{
			get { return "Mock Plugin"; }
		}

		public string Icon
		{
			get { return "Mock.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}
	}
}