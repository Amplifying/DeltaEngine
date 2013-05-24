using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DeltaEngine.Editor.Builder
{
	/// <summary>
	/// The list control which visualizes build messages (infos, errors, warnings) that can occur
	/// during the building process of an application via the BuildService.
	/// </summary>
	public partial class BuildMessagesListView : UserControl
	{
		public BuildMessagesListView()
			: this(new BuildMessagesListViewModel()) {}

		public BuildMessagesListView(BuildMessagesListViewModel messagesViewModel)
		{
			InitializeComponent();
			MessagesViewModel = messagesViewModel;
		}

		public BuildMessagesListViewModel MessagesViewModel
		{
			get
			{
				return messagesViewModel;
			}
			set
			{
				messagesViewModel = value;
				DataContext = messagesViewModel;
				UpdateErrorsStackPanelBackground();
				UpdateWarningsStackPanelBackground();
				UpdateMessagesStackPanelBackground();
			}
		}
		private BuildMessagesListViewModel messagesViewModel;

		private void UpdateErrorsStackPanelBackground()
		{
			Color brushColor = MessagesViewModel.IsShowingErrorsAllowed ?
				ToggleEnabledColor :
				ToggleDisabledColor;
			ErrorsStackPanel.Background = new SolidColorBrush(brushColor);
		}

		private static readonly Color ToggleEnabledColor = Colors.SteelBlue;
		private static readonly Color ToggleDisabledColor = Colors.DimGray;

		private void UpdateWarningsStackPanelBackground()
		{
			Color brushColor = MessagesViewModel.IsShowingWarningsAllowed ?
				ToggleEnabledColor :
				ToggleDisabledColor;
			WarningsStackPanel.Background = new SolidColorBrush(brushColor);
		}

		private void UpdateMessagesStackPanelBackground()
		{
			Color brushColor = MessagesViewModel.IsShowingInfosAllowed ?
				ToggleEnabledColor :
				ToggleDisabledColor;
			MessagesStackPanel.Background = new SolidColorBrush(brushColor);
		}

		private void UpdateMessageColumnWidth(object sender, SizeChangedEventArgs e)
		{
			// We need to resize the grid columns ourselves as WPF does not support this feature (Auto
			// will just always resize to the content, which is not what we want). A better and more
			// complete solution can be found here:
			// http://www.codeproject.com/KB/grid/ListView_layout_manager.aspx
			// However, this is too much work to implement right now!
			double widthOfOtherColumns = IconGridViewColumn.Width + TimeGridViewColumn.Width +
				ProjectGridViewColumn.Width + FileGridViewColumn.Width;
			const double ColumnBorderCorrection = 10;
			double newMessageColumnWidth = (ActualWidth - ColumnBorderCorrection) - widthOfOtherColumns;
			if (newMessageColumnWidth < 50)
				newMessageColumnWidth = 50;
			MessageGridViewColumn.Width = newMessageColumnWidth;
		}

		private void OnOpenDoubleClickedFile(object sender, MouseButtonEventArgs e)
		{
			// TODO when possible
		}

		private void OnErrorsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			MessagesViewModel.IsShowingErrorsAllowed = !MessagesViewModel.IsShowingErrorsAllowed;
			UpdateErrorsStackPanelBackground();
		}

		private void OnWarningsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			MessagesViewModel.IsShowingWarningsAllowed = !MessagesViewModel.IsShowingWarningsAllowed;
			UpdateWarningsStackPanelBackground();
		}

		private void OnMessagesStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			MessagesViewModel.IsShowingInfosAllowed = !MessagesViewModel.IsShowingInfosAllowed;
			UpdateMessagesStackPanelBackground();
		}
	}
}