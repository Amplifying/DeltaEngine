using DeltaEngine.Editor.Common;

namespace DeltaEngine.Editor.ParticleEditor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class ParticleEditorView : EditorPluginView
	{
		public ParticleEditorView(Service service) : this()
		{
			
		}

		public ParticleEditorView()
		{

		}

		public string ShortName
		{
			get { return "Particle Editor"; }
		}
		public string Icon
		{
			get { return "Icons/ParticleEffect.png"; }
		}
		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Particles; }
		}
		public int Priority
		{
			get { return 1; }
		}
	}
}
