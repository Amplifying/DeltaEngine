using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using DeltaEngine.Editor.Common;
using FormsPanel = System.Windows.Forms.Panel;
using DrawingColors = System.Drawing.Color;

namespace DeltaEngine.Editor.Viewport
{
	/// <summary>
	/// The only link from the editor to the engine to display some awesome stuff
	/// </summary>
	public partial class RenderControl : EditorPluginView
	{
		public RenderControl(Service service)
		{
			InitializeComponent();
		}

		public string ShortName
		{
			get { return "Start"; }
		}
		public string Icon
		{
			get { return "Icons/Start.png"; }
		}
		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.GettingStarted; }
		}
		public int Priority
		{
			get { return 1; }
		}

		private void ChangeSize(object sender, SizeChangedEventArgs e) {}
	}
}