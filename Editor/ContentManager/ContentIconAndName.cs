using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.ContentManager
{
	public class ContentIconAndName : ViewModelBase
	{
		public ContentIconAndName(string icon, string name,
			ObservableCollection<ContentIconAndName> subContent = null)
		{
			Icon = icon;
			Name = name;
			SubContent = subContent ?? new ObservableCollection<ContentIconAndName>();
			Brush = new SolidColorBrush();
		}

		public string Icon { get; set; }
		public string Name { get; set; }
		public Brush Brush
		{
			get { return brush; }
			set
			{
				brush = value;
				RaisePropertyChanged("Brush");
			}
		}

		private Brush brush;
		public ObservableCollection<ContentIconAndName> SubContent { get; set; }
	}
}