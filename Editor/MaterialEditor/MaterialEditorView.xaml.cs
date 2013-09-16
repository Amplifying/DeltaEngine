using System.Windows;
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
			curent = new MaterialEditorViewModel(service);
			this.service = service;
			DataContext = curent;
		}

		private MaterialEditorViewModel curent;
		private Service service;

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
			curent.Save();
		}

		private void OpenAnimationEditor(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(AnimationEditorView));
		}
	}
}