using System.Windows;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.ParticleEditor
{
	/// <summary>
	/// Interaction logic for ParticleEditorView.xaml
	/// </summary>
	public partial class ParticleEditorView : EditorPluginView
	{
		public ParticleEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			current = new ParticleEditorViewModel(service);
			DataContext = current;
		}

		private ParticleEditorViewModel current;

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
			current.Save();
		}
	}
}