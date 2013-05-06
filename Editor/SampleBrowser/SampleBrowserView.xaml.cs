using System.Windows.Input;
using DeltaEngine.Editor.Common;

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
			Loaded += (sender, args) => Init();
		}

		public SampleBrowserView(Service service)
			: this() {}

		private void Init()
		{
			if (DataContext is SampleBrowserViewModel)
				return;

			var model = new SampleBrowserViewModel();
			DataContext = model;
			model.GetSamples();
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
			get { return "Icons/Samples.png"; }
		}
		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.GettingStarted; }
		}
		public int Priority
		{
			get { return 3; }
		}
	}
}