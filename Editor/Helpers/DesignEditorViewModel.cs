using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Logging;

namespace DeltaEngine.Editor.Helpers
{
	public class DesignEditorViewModel
	{
		public DesignEditorViewModel()
		{
			IsLoggedIn = true;
			ApiKey = "1A2B3C4D-5E6F-7G8H-9I0J-1K2L3M4N5O6P";
			Error = "Error message";
			TextLogger = new TextLogger();
			TextLogger.Write(Logger.MessageType.Info, "Log message");
			Service = new MockService("exDreamDuck", "Game - The Game");
			AvailableProjects = new List<string> { "Project One", "Project Two", "Project Three" };
			SelectedProject = AvailableProjects[0];
		}

		public bool IsLoggedIn { get; private set; }
		public string ApiKey { get; set; }
		public string Error { get; private set; }
		public TextLogger TextLogger { get; private set; }
		public MockService Service { get; private set; }
		public List<string> AvailableProjects { get; private set; }
		public string SelectedProject { get; set; }

		public Visibility LoginPanelVisibility
		{
			get { return IsLoggedIn ? Visibility.Hidden : Visibility.Visible; }
		}

		public Visibility ErrorVisibility
		{
			get { return Error != "" ? Visibility.Visible : Visibility.Hidden; }
		}

		public Visibility EditorPanelVisibility
		{
			get { return IsLoggedIn ? Visibility.Visible : Visibility.Hidden; }
		}
	}
}