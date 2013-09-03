using System.Windows;
using DeltaEngine.Editor.Core;
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
			DataContext = new UIEditorViewModel(service);
		}

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

		private void RemoveImage(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("RemoveSprite", "RemoveSprite");
		}

		private void SaveUI(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("SaveUI", "SaveUI");
		}
	}
}