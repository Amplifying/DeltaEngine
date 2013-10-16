using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// Interaction logic for ContentIconAndNameView.xaml
	/// </summary>
	public partial class ContentIconAndNameView
	{
		//ncrunch: no coverage start
		public ContentIconAndNameView()
		{
			InitializeComponent();
		}

		private void ClickOnElement(object sender, MouseButtonEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.LeftCtrl))
				Messenger.Default.Send(ContentName.Text, "AddToSelection");
			else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				Messenger.Default.Send(ContentName.Text, "AddMultipleContentToSelection");
			else
			{
				Messenger.Default.Send("ClearList", "ClearList");
				Messenger.Default.Send(ContentName.Text, "AddToSelection");
			}
		}
	}
}