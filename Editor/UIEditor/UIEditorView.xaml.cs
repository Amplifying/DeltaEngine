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
			DataContext = new UIEditorViewModel();
		}

		public void Init(Service service) {}

		public string ShortName
		{
			get { return "UI Screens"; }
		}

		public string Icon
		{
			get { return "Icons/UI.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void AddImage(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("AddImage", "AddImage");
		}
	}
}