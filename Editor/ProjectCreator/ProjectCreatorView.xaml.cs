using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Enables users to create a Delta Engine project and select framework (Xna, OpenGL, DirectX)
	/// </summary>
	public partial class ProjectCreatorView : EditorPluginView
	{
		public ProjectCreatorView()
			: this(new ProjectCreatorViewModel()) {}

		public ProjectCreatorView(ProjectCreatorViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			this.viewModel = viewModel;
		}

		private readonly ProjectCreatorViewModel viewModel;

		public void Init(Service service)
		{
			viewModel.Service = service;
		}

		public void ProjectChanged() {}

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
			get { return false; }
		}
	}
}