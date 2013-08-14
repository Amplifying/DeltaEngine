using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// The list control which shows the list of already built apps or the warnings and errors of the
	/// app which will be currently build.
	/// </summary>
	public partial class AppBuilderInfoListView
	{
		public AppBuilderInfoListView()
		{
			InitializeComponent();
		}

		public BuiltAppsListViewModel AppListViewModel
		{
			get { return BuiltAppsList.ViewModel; }
			set
			{
				BuiltAppsList.ViewModel = value;
				AppsLabel.DataContext = value;
				FocusBuiltAppsList();
			}
		}

		public void FocusBuiltAppsList()
		{
			MessagesViewModel.IsShowingErrorsAllowed = false;
			MessagesViewModel.IsShowingWarningsAllowed = false;
			BuildMessagesList.Visibility = Visibility.Collapsed;
			BuiltAppsList.Visibility = Visibility.Visible;
			AppsLabel.Background = new SolidColorBrush(ToggleEnabledColor);
		}

		public AppBuildMessagesListViewModel MessagesViewModel
		{
			get { return BuildMessagesList.ViewModel; }
			set
			{
				if (BuildMessagesList.ViewModel != null)
					BuildMessagesList.ViewModel.PropertyChanged -= OnMessagesListPropertyChanged;
				BuildMessagesList.ViewModel = value;
				BuildMessagesList.ViewModel.PropertyChanged += OnMessagesListPropertyChanged;
				ErrorsLabel.DataContext = value;
				WarningsLabel.DataContext = value;
				FocusBuildMessagesList();
			}
		}

		private void OnMessagesListPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsShowingErrorsAllowed")
				UpdateErrorsStackPanelBackground();
			if (e.PropertyName == "IsShowingWarningsAllowed")
				UpdateWarningsStackPanelBackground();
		}

		private void UpdateErrorsStackPanelBackground()
		{
			Color brushColor = MessagesViewModel.IsShowingErrorsAllowed ?
				ToggleEnabledColor :
				ToggleDisabledColor;
			ErrorsLabel.Background = new SolidColorBrush(brushColor);
		}

		private static readonly Color ToggleEnabledColor =
			(Color)ColorConverter.ConvertFromString("#FFF0F0F0");
		private static readonly Color ToggleDisabledColor = Colors.Gray;

		private void UpdateWarningsStackPanelBackground()
		{
			Color brushColor = MessagesViewModel.IsShowingWarningsAllowed ?
				ToggleEnabledColor :
				ToggleDisabledColor;
			WarningsLabel.Background = new SolidColorBrush(brushColor);
		}

		private void OnErrorsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			if (IsBuildMessagesListFocused)
				MessagesViewModel.IsShowingErrorsAllowed = !MessagesViewModel.IsShowingErrorsAllowed;
			else
				FocusBuildMessagesList();
		}

		public bool IsBuildMessagesListFocused
		{
			get { return BuildMessagesList.Visibility == Visibility.Visible; }
		}

		public void FocusBuildMessagesList()
		{
			BuiltAppsList.Visibility = Visibility.Collapsed;
			AppsLabel.Background = new SolidColorBrush(ToggleDisabledColor);
			BuildMessagesList.Visibility = Visibility.Visible;
			MessagesViewModel.IsShowingErrorsAllowed = true;
			MessagesViewModel.IsShowingWarningsAllowed = true;
		}

		private void OnWarningsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			if (IsBuildMessagesListFocused)
				MessagesViewModel.IsShowingWarningsAllowed = !MessagesViewModel.IsShowingWarningsAllowed;
			else
				FocusBuildMessagesList();
		}

		private void OnPlatformsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			if (IsBuiltAppsListFocused)
				return;

			FocusBuiltAppsList();
		}

		public bool IsBuiltAppsListFocused
		{
			get { return BuiltAppsList.Visibility == Visibility.Visible; }
		}
	}
}
