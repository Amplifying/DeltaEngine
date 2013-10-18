using System.Collections.Generic;
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
			var namesAndPaths = new UIToolNamesAndPaths();
			foreach (string tool in namesAndPaths.GetNames())
				Tools.Add(new ToolboxEntry(tool, namesAndPaths.GetImagePath(tool)));
		}
	}
}