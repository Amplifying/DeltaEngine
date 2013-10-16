using System.Collections.Generic;
using DeltaEngine.Editor.UIEditor;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.Emulator
{
	public class ViewportControlViewModel : ViewModelBase
	{
		public ViewportControlViewModel()
		{
			Tools = new List<ToolboxEntry>();
			SetupToolbox();
		}

		public List<ToolboxEntry> Tools { get; private set; }

		private void SetupToolbox()
		{
			foreach (UITool tool in UIToolExtensions.GetNames())
				Tools.Add(new ToolboxEntry(tool.ToString(), tool.GetImagePath()));
		}
	}
}