using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering.Sprites;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIEditorViewModel : ViewModelBase
	{
		public UIEditorViewModel()
		{
			ContentPath = "Content";
			ProjectList = new ObservableCollection<string>();
			UIImagesInList = new ObservableCollection<string>();
			Messenger.Default.Register<string>(this, "AddImage", AddImage);
		}

		public ObservableCollection<string> ProjectList { get; set; }
		public ObservableCollection<string> UIImagesInList { get; private set; }
		public string ContentPath { get; private set; }

		public void AddImage(string image)
		{
			if (string.IsNullOrEmpty(SelectedImageInList))
				return;

			AddNewImageToList(image);
			RaisePropertyChanged("UIImages");
			RaisePropertyChanged("ImageInGridList");
		}

		public string SelectedImageInList { get; set; }

		public void AddNewImageToList(string newImage)
		{
			new Sprite(new Material(Shader.Position2DUv, newImage), Rectangle.One);
		}
	}
}
