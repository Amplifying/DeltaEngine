using System.Windows;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.MaterialEditor;

namespace DeltaEngine.Editor.ParticleEditor
{
	/// <summary>
	/// Interaction logic for ParticleEditorView.xaml
	/// </summary>
	public partial class ParticleEditorView : EditorPluginView
	{
		//ncrunch: no coverage start, relating arguments only
		public ParticleEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service setService)
		{
			viewModel = new ParticleEditorViewModel(setService);
			service = setService;
			DataContext = viewModel;
		}

		private ParticleEditorViewModel viewModel;
		private Service service;

		public void ProjectChanged() {}

		public string ShortName
		{
			get { return "Particle Effects"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/ParticleEffect.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			viewModel.Save();
		}

		private void OpenMaterialEditor(object sender, RoutedEventArgs e)
		{
			service.StartPlugin(typeof(MaterialEditorView));
		}

		private void Delete(object sender, RoutedEventArgs e)
		{
			viewModel.DeleteParticleContent();
		}
	}
}