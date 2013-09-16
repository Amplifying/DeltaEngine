using System.Windows;
using System.Windows.Controls;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.MaterialEditor;
using DeltaEngine.Editor.SpriteFontCreator;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	/// <summary>
	/// Interaction logic for UIEditorView.xaml
	/// </summary>
	public partial class UIEditorView : EditorPluginView
	{
		public UIEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			this.service = service;
			DataContext = new UIEditorViewModel(service);
		}

		private Service service;

		public string ShortName
		{
			get { return "UI Screens"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/UI.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void AddImage(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("AddImage", "AddImage");
		}

		private void SaveUI(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("SaveUI", "SaveUI");
		}

		private void ChangeMaterial(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems[0] == null)
				return;
			Messenger.Default.Send(e.AddedItems[0].ToString(), "ChangeMaterial");
		}

		private void OpenMaterialEditorClick(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(MaterialEditorView));
		}

		private void OpenFontEditor(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(FontCreatorView));
		}

		public void IncreaseRenderlayer(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(1, "ChangeRenderLayer");
		}

		public void DecreaseRenderLayer(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(-1, "ChangeRenderLayer");
		}
	}
}