using System;
using System.Timers;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor
{
	public class PopupMessageViewModel
	{
		public PopupMessageViewModel(Service service, int messageDisplayTime = 3000)
		{
			Text = "";
			Visiblity = Visibility.Hidden;
			this.messageDisplayTime = messageDisplayTime;
			service.ContentUpdated += ShowUpdateText;
		}

		public string Text { get; private set; }
		public Visibility Visiblity { get; private set; }
		private readonly int messageDisplayTime;

		private void ShowUpdateText(ContentType type, string name)
		{
			Visiblity = Visibility.Visible;
			Text = name + " " + type + " Updated!";
			messageVisibility = new Timer(messageDisplayTime);
			messageVisibility.Elapsed += RemoveUpdateText;
			messageVisibility.Start();
			FireVisibilityUpdatedAction();
		}

		private Timer messageVisibility;

		private void FireVisibilityUpdatedAction()
		{
			if (MessageUpdated != null)
				MessageUpdated();
		}

		public event Action MessageUpdated;

		private void RemoveUpdateText(object sender, ElapsedEventArgs e)
		{
			Visiblity = Visibility.Hidden;
			messageVisibility.Elapsed -= RemoveUpdateText;
			messageVisibility.Dispose();
			FireVisibilityUpdatedAction();
		}
	}
}