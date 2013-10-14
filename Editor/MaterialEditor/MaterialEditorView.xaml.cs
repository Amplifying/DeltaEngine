using System.Windows;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.ImageAnimationEditor;

namespace DeltaEngine.Editor.MaterialEditor
{
	/// <summary>
	/// Interaction logic for MaterialEditorView.xaml
	/// </summary>
	public partial class MaterialEditorView : EditorPluginView
	{
		public MaterialEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			current = new MaterialEditorViewModel(service);
			this.service = service;
			DataContext = current;
		}

		private MaterialEditorViewModel current;
		private Service service;

		public void ProjectChanged() {}

		public string ShortName
		{
			get { return "Material Editor"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Material.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		private void SaveMaterial(object sender, RoutedEventArgs e)
		{
			current.Save();
		}

		private void OpenAnimationEditor(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(AnimationEditorView));
		}

		private void ButtonBaseOnClick(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(ContentManagerView));
		}
	}
}