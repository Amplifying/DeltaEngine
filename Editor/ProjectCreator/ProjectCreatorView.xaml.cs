using System.Windows;
using System.Windows.Forms;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Enables users to create a Delta Engine project and select framework (Xna, OpenGL, DirectX)
	/// </summary>
	public partial class ProjectCreatorView : EditorPluginView
	{
		public ProjectCreatorView()
		{
			InitializeComponent();
		}

		public ProjectCreatorView(ProjectCreatorViewModel viewModel)
		{
			DataContext = this.viewModel = viewModel;
		}

		public void Init(Service service)
		{
			DataContext = viewModel = new ProjectCreatorViewModel(service);
			Messenger.Default.Send("ProjectCreator", "SetSelectedEditorPlugin");
		}

		public void Activate()
		{
			Messenger.Default.Send("ProjectCreator", "SetSelectedEditorPlugin");
		}

		private ProjectCreatorViewModel viewModel;

		public string ShortName
		{
			get { return "Project Creator"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/ProjectCreator.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}

		private void OnBrowseUserProjectsClicked(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
				viewModel.Location = dialog.SelectedPath;
		}
	}
}