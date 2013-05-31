using System;
using System.Windows.Forms;
using System.Windows.Media;
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
			ViewportHoster.Background = new SolidColorBrush(Colors.Aqua);
			var panel = new FormsPanel { Name = "Hi", BackColor = DrawingColors.Blue };
			ViewportHoster.Child = panel;
			var button = new Button { Text = "Hello" };
			button.Click += delegate { MessageBox.Show("Button clicked"); };
			panel.Controls.Add(button);
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
	}
}