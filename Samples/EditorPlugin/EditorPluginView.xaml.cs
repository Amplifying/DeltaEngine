namespace DeltaEngine.Editor.EditorPlugin
{
	/// <summary>
	/// Code-behind of EditorPluginView.xaml
	/// </summary>
	public partial class EditorPluginView
	{
		public EditorPluginView()
			: this(new EditorPluginViewModel()) {}

		public EditorPluginView(EditorPluginViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
		}
	}
}