using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.ContentManager
{
	public class ContentViewModel : ViewModelBase
	{
		public ContentViewModel(string icon, string contentName)
		{
			Icon = icon;
			ContentName = contentName;
		}
		public string Icon { get; set; }
		public string ContentName { get; set; }
	}
}
