using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.EmptyEditorPlugin
{
	/// <summary>
	/// Code-behind of EmptyEditorPluginView.xaml
	/// </summary>
	public partial class EmptyEditorPluginView : EditorPluginView
	{
		public EmptyEditorPluginView()
		{
			InitializeComponent();
			Loaded += (sender, args) => Init();
		}

		public EmptyEditorPluginView(Service service)
			: this() {}

		private void Init()
		{
			if (DataContext is EmptyEditorPluginViewModel)
				return;

			DataContext = new EmptyEditorPluginViewModel();
		}

		public string ShortName
		{
			get { return "EmptyEditorPlugin"; }
		}

		public string Icon
		{
			get { return "Icons/New.png"; }
		}

		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.GettingStarted; }
		}

		public int Priority
		{
			get { return 1; }
		}
	}
}