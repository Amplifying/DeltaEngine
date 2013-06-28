using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DeltaEngine.Editor.Builder
{
	/// <summary>
	/// Interaction logic for BuilderInfoListView.xaml
	/// </summary>
	public partial class BuilderInfoListView : UserControl
	{
		public BuilderInfoListView()
		{
			InitializeComponent();
		}

		public BuildMessagesListViewModel MessagesViewModel
		{
			get { return BuildMessagesList.DataContext as BuildMessagesListViewModel; }
			set { BuildMessagesList.DataContext = value; }
		}

		public BuiltAppsListViewModel AppListViewModel
		{
			get { return BuiltAppsList.DataContext as BuiltAppsListViewModel; }
			set { BuiltAppsList.DataContext = value; }
		}

		private void OnErrorsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			if (IsBuildMessagesListFocused)
				ToggleShowingBuildErrorMessages();
			else
				FocusBuildMessagesList();
		}

		public bool IsBuildMessagesListFocused
		{
			get { return BuildMessagesList.Visibility == Visibility.Visible; }
		}

		private void ToggleShowingBuildErrorMessages()
		{
			MessagesViewModel.IsShowingErrorsAllowed = !MessagesViewModel.IsShowingErrorsAllowed;
			UpdateErrorsStackPanelBackground();
		}

		private void UpdateErrorsStackPanelBackground()
		{
			Color brushColor = MessagesViewModel.IsShowingErrorsAllowed ?
				ToggleEnabledColor :
				ToggleDisabledColor;
			ErrorsStackPanel.Background = new SolidColorBrush(brushColor);
		}

		private static readonly Color ToggleEnabledColor = Colors.SteelBlue;
		private static readonly Color ToggleDisabledColor = Colors.DimGray;

		public void FocusBuildMessagesList()
		{
			BuiltAppsList.Visibility = Visibility.Collapsed;
			PlatformsStackPanel.Background = new SolidColorBrush(ToggleDisabledColor);
			BuildMessagesList.Visibility = Visibility.Visible;
			MessagesViewModel.IsShowingErrorsAllowed = true;
			UpdateErrorsStackPanelBackground();
			MessagesViewModel.IsShowingWarningsAllowed = true;
			UpdateWarningsStackPanelBackground();
		}

		private void UpdateWarningsStackPanelBackground()
		{
			Color brushColor = MessagesViewModel.IsShowingWarningsAllowed ?
				ToggleEnabledColor :
				ToggleDisabledColor;
			WarningsStackPanel.Background = new SolidColorBrush(brushColor);
		}

		private void OnWarningsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			if (IsBuildMessagesListFocused)
				ToggleShowingBuildWarningMessages();
			else
				FocusBuildMessagesList();
		}

		private void ToggleShowingBuildWarningMessages()
		{
			MessagesViewModel.IsShowingWarningsAllowed = !MessagesViewModel.IsShowingWarningsAllowed;
			UpdateWarningsStackPanelBackground();
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

		public void FocusBuiltAppsList()
		{
			MessagesViewModel.IsShowingErrorsAllowed = false;
			UpdateErrorsStackPanelBackground();
			MessagesViewModel.IsShowingWarningsAllowed = false;
			UpdateWarningsStackPanelBackground();
			BuildMessagesList.Visibility = Visibility.Collapsed;
			BuiltAppsList.Visibility = Visibility.Visible;
			PlatformsStackPanel.Background = new SolidColorBrush(ToggleEnabledColor);
		}
	}
}
