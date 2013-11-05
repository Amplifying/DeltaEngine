using System.Windows.Input;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Host View for all the Sample items.
	/// </summary>
	public partial class SampleBrowserView : EditorPluginView
	{
		public SampleBrowserView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			if (DataContext is SampleBrowserViewModel)
				return;
			var model = new SampleBrowserViewModel();
			DataContext = model;
			model.UpdateItems();
			Messenger.Default.Send("SampleBrowser", "SetSelectedEditorPlugin");
		}

		public void Activate()
		{
			Messenger.Default.Send("SampleBrowser", "SetSelectedEditorPlugin");
		}

		private void SearchTextBoxGotMouseCapture(object sender, MouseEventArgs e)
		{
			SearchTextBox.SelectAll();
		}

		public string ShortName
		{
			get { return "Sample Browser"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/SampleBrowser.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}
	}
}