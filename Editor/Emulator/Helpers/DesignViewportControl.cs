using System.Collections.Generic;

namespace DeltaEngine.Editor.Emulator.Helpers
{
	public class DesignViewportControl
	{
		public DesignViewportControl()
		{
			Tools = new List<ToolboxEntry>();
			Tools.Add(new ToolboxEntry("Tool 1", "../Images/UIEditor/CreateLabel.png"));
			Tools.Add(new ToolboxEntry("Tool 2", "../Images/UIEditor/CreateRadioDialog.png"));
			Tools.Add(new ToolboxEntry("Tool 3", "../Images/UIEditor/CreateTilemap.png"));
		}

		public List<ToolboxEntry> Tools { get; private set; }
	}
}