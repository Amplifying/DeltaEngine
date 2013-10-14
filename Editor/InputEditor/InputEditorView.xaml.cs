using System.Windows;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.InputEditor
{
	/// <summary>
	/// editor for the input commands
	/// </summary>
	public partial class InputEditorView : EditorPluginView
	{
		public InputEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			DataContext = new InputEditorViewModel(service);
		}

		public void ProjectChanged() {}

		public string ShortName
		{
			get { return "Input Commands"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Input.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Save(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("SaveCommands", "SaveCommands");
		}
	}
}