using System.Windows.Input;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Host View for all the Sample items.
	/// </summary>
	public partial class SampleBrowserView
	{
		public SampleBrowserView()
		{
			InitializeComponent();
			DataContext = new SampleBrowserViewModel();
		}

		private void SearchTextBoxGotMouseCapture(object sender, MouseEventArgs e)
		{
			SearchTextBox.SelectAll();
		}
	}
}