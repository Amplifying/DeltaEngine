using System.Windows;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Common;
using DeltaEngine.Rendering.Shapes;
using Point = DeltaEngine.Datatypes.Point;

namespace DeltaEngine.Editor.UIEditor
{
	/// <summary>
	/// Interaction logic for UIEditorView.xaml
	/// </summary>
	public partial class UIEditorView : EditorPluginView
	{
		public UIEditorView()
		{
			InitializeComponent();
		}

		public UIEditorView(Service service)
			: this() {}

		public string ShortName
		{
			get { return "UI Editor"; }
		}
		public string Icon
		{
			get { return "Icons/UI.png"; }
		}
		public EditorPluginCategory Category
		{
			get { return EditorPluginCategory.Content; }
		}
		public int Priority
		{
			get { return 1; }
		}

		private void OnButtonClicked(object sender, RoutedEventArgs e)
		{
			new Line2D(new Point(0, 1), new Point(Time.Current.Milliseconds / 5000.0f, 0), Color.Yellow);
		}
	}
}