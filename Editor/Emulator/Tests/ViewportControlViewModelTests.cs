using System;
using DeltaEngine.Editor.UIEditor;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class ViewportControlViewModelTests
	{
		[Test, RequiresSTA]
		public void CreateNewViewportControlViewModel()
		{
			var viewModel = new ViewportControlViewModel();
			Assert.AreEqual(Enum.GetNames(typeof(UITool)).Length, viewModel.Tools.Count);
		}
	}
}
