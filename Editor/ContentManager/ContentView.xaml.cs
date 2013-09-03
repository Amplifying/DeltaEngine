using System.Windows.Controls;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// Interaction logic for ContentView.xaml
	/// </summary>
	public partial class ContentView
	{
		public ContentView()
		{
			InitializeComponent();			
		}

		public void CreateContentView(string icon, string contentName)
		{
			NewContent = new ContentViewModel(icon, contentName);
			DataContext = NewContent;
		}

		public ContentViewModel NewContent;
	}
}
