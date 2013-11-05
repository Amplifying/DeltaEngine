using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DeltaEngine.Editor.ContentManager
{
	public class DesignContentManagerView
	{
		public DesignContentManagerView()
		{
			var children = new ObservableCollection<ContentIconAndName>();
			children.Add(new ContentIconAndName("../Images/ContentTypes/Xml.png", "Xml File"));
			ContentList = new List<ContentIconAndName>
			{
				new ContentIconAndName("../Images/ContentTypes/Image.png", "Test Image", children),
				new ContentIconAndName("../Images/ContentTypes/Json.png", "Json File")
			};
		}

		public List<ContentIconAndName> ContentList { get; set; }
	}
}